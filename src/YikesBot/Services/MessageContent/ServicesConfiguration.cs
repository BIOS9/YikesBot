using Microsoft.Extensions.DependencyInjection;
using YikesBot.Services.MessageContent.ContentHandlers;

namespace YikesBot.Services.MessageContent;

public static class ServicesConfiguration
{
    public static IServiceCollection AddMessageContentHandler(this IServiceCollection services)
    {
        services.AddSingleton<MessageContentHandler>();
        services.AddScoped<IContentHandler, AwooHandler>();
        return services;
    }
}