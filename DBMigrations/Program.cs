using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var environmentName = Environment.GetCommandLineArgs()[1];
environmentName = "Release";
IConfigurationRoot? configuration = null;
try
{
    configuration = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.{environmentName}.json")
        .Build();
}
catch (FileNotFoundException)
{
    Console.WriteLine($"Не сужествует файл appsettings.{environmentName}.json. Укажите правильное окружение (Development/Production)");
}

var postgesConnectionString = configuration!.GetConnectionString("Postgres");

using var serviceProvider = new ServiceCollection()
    .AddFluentMigratorCore()
    .ConfigureRunner(configuration =>
        configuration
            .AddPostgres()
            .WithGlobalConnectionString(postgesConnectionString)
            .ScanIn(typeof(Program).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole())
    .BuildServiceProvider(false);

using var scope = serviceProvider.CreateScope();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

runner.MigrateDown(0);
runner.MigrateUp();