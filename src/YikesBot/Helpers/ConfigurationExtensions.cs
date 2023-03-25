using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace YikesBot.Helpers;

public static class ConfigurationHelpers
{
    public static IConfigurationSection GetExistingSectionOrThrow(this IConfiguration configuration, string key)
    {
        var configurationSection = configuration.GetSection(key);

        if (!configurationSection.Exists())
            throw configuration switch
            {
                IConfigurationRoot configurationIsRoot => new ArgumentException(
                    $"Section with key '{key}' does not exist. Existing values are: {configurationIsRoot.GetDebugView()}",
                    nameof(key)),
                IConfigurationSection configurationIsSection => new ArgumentException(
                    $"Section with key '{key}' does not exist at '{configurationIsSection.Path}'. Expected configuration path is '{configurationSection.Path}'",
                    nameof(key)),
                _ => new ArgumentException($"Failed to find configuration at '{configurationSection.Path}'",
                    nameof(key))
            };

        return configurationSection;
    }

    public static IServiceCollection ConfigureWithValidation<TOptions>(this IServiceCollection services,
        IConfiguration config) where TOptions : class
    {
        return services.ConfigureWithValidation<TOptions>(Options.DefaultName, config);
    }

    public static IServiceCollection ConfigureWithValidation<TOptions>(this IServiceCollection services, string name,
        IConfiguration config) where TOptions : class
    {
        _ = config ?? throw new ArgumentNullException(nameof(config));
        services.Configure<TOptions>(name, config);
        services.AddDataAnnotationValidatedOptions<TOptions>(name);
        return services;
    }

    public static IServiceCollection ConfigureWithValidation<TOptions>(this IServiceCollection services,
        Action<TOptions> configureOptions) where TOptions : class
    {
        return services.ConfigureWithValidation(Options.DefaultName, configureOptions);
    }

    public static IServiceCollection ConfigureWithValidation<TOptions>(this IServiceCollection services, string name,
        Action<TOptions> configureOptions) where TOptions : class
    {
        services.Configure(name, configureOptions);
        services.AddDataAnnotationValidatedOptions<TOptions>(name);
        return services;
    }

    private static IServiceCollection AddDataAnnotationValidatedOptions<TOptions>(this IServiceCollection services,
        string name) where TOptions : class
    {
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IValidateOptions<TOptions>>(new DataAnnotationValidateOptions<TOptions>(name)));
        return services;
    }
}