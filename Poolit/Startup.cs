using Amazon.S3;

namespace WebApplication1;

public class Startup
{
    /*public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void CofigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddAWSService<IAmazonS3>();
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
 
        app.UseRouting();
 
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        });
    }*/
}