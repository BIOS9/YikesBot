using Microsoft.Extensions.DependencyInjection;
using YikesBot.Services.SlashCommands;
using YikesBot.Services.SlashCommands.Commands;

namespace YikesBot.Services.SlashCommands;

public static class ServicesConfiguration
{
    public static IServiceCollection AddSlashCommands(this IServiceCollection services)
    {
        services.AddSingleton<SlashCommandHandler>();
        services.AddScoped<ICommand, PurgeCommand>();
        return services;
    }
}