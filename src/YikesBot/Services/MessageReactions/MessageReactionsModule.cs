using Autofac;
using Microsoft.Extensions.Hosting;
using YikesBot.Services.MessageReactions.ReactionHandlers;

namespace YikesBot.Services.MessageReactions;

public class MessageReactionsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MessageReactionHandler>()
            .AsSelf()
            .As<IHostedService>()
            .SingleInstance();
        builder.RegisterType<PinReactHandler>().As<IReactionHandler>();
    }
}