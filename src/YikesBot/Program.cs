using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using YikesBot;
using YikesBot.Services.Bot;
using YikesBot.Services.DeletedMessages;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddEnvironmentVariables();
        config.AddUserSecrets<Program>();
    })
    .UseSerilog((context, config) =>
    {
        config.ReadFrom.Configuration(context.Configuration);
    })
    .ConfigureServices((context, services) =>
    {
        services
            .AddHostedService<Startup>()
            .AddDiscordBot(context.Configuration)
            .AddDeletedMessagesLogger();
    })
    .Build()
    .RunAsync();