using Autofac;

namespace YikesBot.Services.DeletedMessages;

public class DeletedMessagesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DeletedMessagesLogger>().SingleInstance();
    }
}