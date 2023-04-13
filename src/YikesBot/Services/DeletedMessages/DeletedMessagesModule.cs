using Autofac;
using Microsoft.Extensions.Hosting;

namespace YikesBot.Services.DeletedMessages;

public class DeletedMessagesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DeletedMessagesLogger>()
            .AsSelf()
            .As<IHostedService>()
            .SingleInstance();
    }
}