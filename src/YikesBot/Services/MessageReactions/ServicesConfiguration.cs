using Microsoft.Extensions.DependencyInjection;
using YikesBot.Services.MessageReactions.ReactionHandlers;

namespace YikesBot.Services.MessageReactions;

public static class ServicesConfiguration
{
    public static IServiceCollection AddMessageReactionHandler(this IServiceCollection services)
    {
        services.AddSingleton<MessageReactionHandler>();
        services.AddScoped<IReactionHandler, PinReactHandler>();
        return services;
    }
}