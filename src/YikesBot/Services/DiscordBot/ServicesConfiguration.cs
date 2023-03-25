using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YikesBot.Helpers;

namespace YikesBot.Services.DiscordBot;

public static class ServicesConfiguration
{
    public static IServiceCollection AddDiscordBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureWithValidation<DiscordBotOptions>(configuration.GetExistingSectionOrThrow(DiscordBotOptions.Name));
        services.AddHostedService<DiscordBot>();
        return services;
    }
    
    public static IServiceCollection AddDiscordBot(this IServiceCollection services, Action<DiscordBotOptions> configureDelegate)
    {
        services.ConfigureWithValidation(configureDelegate);
        services.AddHostedService<DiscordBot>();
        return services;
    }
}