using Microsoft.Extensions.Hosting;
using Serilog;
using YikesBot.Services.DiscordBot;

await Host.CreateDefaultBuilder(args)
    .UseSerilog((context, config) =>
    {
        config.ReadFrom.Configuration(context.Configuration);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDiscordBot(context.Configuration);
    })
    .Build()
    .RunAsync();