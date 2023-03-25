using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        
    })
    .ConfigureAppConfiguration(appConfig =>
    {
        appConfig.AddEnvironmentVariables();
        appConfig.AddUserSecrets<Program>();
    })
    .UseSerilog()
    .Build()
    .RunAsync();
    