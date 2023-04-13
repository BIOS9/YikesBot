using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using YikesBot.Helpers;

namespace YikesBot.Services.Bot;

public class BotModule : Module
{
    private readonly IConfiguration _configuration;

    public BotModule(IConfiguration configuration)
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