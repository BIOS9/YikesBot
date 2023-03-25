using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        
    })
    .ConfigureAppConfiguration(appConfig =>
    {
        appConfig.AddEnvironmentVariables();
        appConfig.AddUserSecrets<Program>();
    })
    .Build()
    .RunAsync();
    