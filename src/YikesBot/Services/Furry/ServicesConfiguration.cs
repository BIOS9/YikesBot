using Microsoft.Extensions.DependencyInjection;

namespace YikesBot.Services.Furry;

public static class ServicesConfiguration
{
    public static IServiceCollection AddFurrySpeaker(this IServiceCollection services)
    {
        services.AddSingleton<FurrySpeaker>();
        return services;
    }
}