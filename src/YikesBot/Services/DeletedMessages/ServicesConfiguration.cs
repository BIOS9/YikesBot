﻿using Microsoft.Extensions.DependencyInjection;

namespace YikesBot.Services.DeletedMessages;

public static class ServicesConfiguration
{
    public static IServiceCollection AddDeletedMessagesLogger(this IServiceCollection services)
    {
        services.AddTransient<DeletedMessagesLogger>();
        return services;
    }
}