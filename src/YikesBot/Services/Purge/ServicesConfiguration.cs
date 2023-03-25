using Microsoft.Extensions.DependencyInjection;
using YikesBot.Services.Bot;

namespace YikesBot.Services.Purge;

public static class ServicesConfiguration
{
    public static IServiceCollection AddPurge(this IServiceCollection services)
    {
        services.AddScoped<ICommand, PurgeCommand>();
        return services;
    }
}