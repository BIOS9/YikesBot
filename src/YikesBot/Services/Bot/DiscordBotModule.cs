using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YikesBot.Helpers;

namespace YikesBot.Services.Bot;

public class DiscordBotModule : Module
{
    private readonly IConfiguration _configuration;

    public DiscordBotModule(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DiscordBot>().AsSelf().As<IHostedService>().SingleInstance().PropertiesAutowired();
        builder.ConfigureWithValidation<DiscordBotOptions>(
            _configuration.GetExistingSectionOrThrow(DiscordBotOptions.Name));
    }
}