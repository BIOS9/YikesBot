using Autofac;

namespace YikesBot.Services.Furry;

public class FurryModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FurrySpeaker>().SingleInstance();
    }
}