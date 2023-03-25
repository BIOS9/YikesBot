using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YikesBot.Helpers;

namespace YikesBot.Services.Bot;

public static class ServicesConfiguration
{
    public static IServiceCollection AddDiscordBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureWithValidation<DiscordBotOptions>(
            configuration.GetExistingSectionOrThrow(DiscordBotOptions.Name));
        services.AddSingleton<DiscordBot>();
        return services;
    }

    public static IServiceCollection AddDiscordBot(this IServiceCollection services,
        Action<DiscordBotOptions> configureDelegate)
    {
        services.ConfigureWithValidation(configureDelegate);
        services.AddSingleton<DiscordBot>();
        return services;
    }
}