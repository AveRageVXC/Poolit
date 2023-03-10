using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Poolit.Services;
using Poolit.Services.Interfaces;

namespace Poolit;

public class Startup
{
    private bool InDocker => Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IFileService, FileService>();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "API",
                Version = "v1",
                Description = "API",
            });
            var filePath = Path.Combine(AppContext.BaseDirectory, "Poolit.xml");
            options.IncludeXmlComments(filePath);
        });

        services.AddControllers().AddNewtonsoftJson(options =>
       options.SerializerSettings.Converters.Add(new StringEnumConverter()));
        services.AddControllers().AddNewtonsoftJson();

        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = int.MaxValue;
        });

        services.Configure<FormOptions>(x =>
        {
            x.ValueLengthLimit = int.MaxValue;
            x.MultipartBodyLengthLimit = int.MaxValue;
            x.MultipartHeadersLengthLimit = int.MaxValue;
        });
        services.AddMvc();

        services.Configure<TokensConfiguration>(Configuration.GetSection("Tokens"));

        //services.AddSwaggerGenNewtonsoftSupport();
        //   services.Configure<Configuration>(Configuration.GetSection("ConnectionStrings"));
        /*if (InDocker)
        {
            Database.ConnectionString = Configuration.GetConnectionString("DatabaseInDocker");
        }
        else
        {
            Database.ConnectionString = Configuration.GetConnectionString("Database");
        }*/
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment() || env.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = "swagger";
            });
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
