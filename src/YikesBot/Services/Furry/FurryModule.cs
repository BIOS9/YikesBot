using Autofac;
using Microsoft.Extensions.Hosting;

namespace YikesBot.Services.Furry;

public class FurryModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FurrySpeaker>()
            .AsSelf()
            .As<IHostedService>()
            .SingleInstance();
    }
}