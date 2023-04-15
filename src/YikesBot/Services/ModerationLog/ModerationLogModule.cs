using Autofac;
using Microsoft.Extensions.Hosting;

namespace YikesBot.Services.ModerationLog;

public class ModerationLogModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ModerationLogger>()
            .AsSelf()
            .As<IHostedService>()
            .SingleInstance();
    }
}