using Microsoft.Extensions.DependencyInjection;

namespace YikesBot.Services.MessageContent;

public static class ServicesConfiguration
{
    public static IServiceCollection AddMessageContentHandler(this IServiceCollection services)
    {
        services.AddSingleton<MessageContentHandler>();
        return services;
    }
}