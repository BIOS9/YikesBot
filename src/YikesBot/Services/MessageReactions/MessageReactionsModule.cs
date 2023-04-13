using Autofac;
using YikesBot.Services.MessageReactions.ReactionHandlers;

namespace YikesBot.Services.MessageReactions;

public class MessageReactionsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MessageReactionHandler>().SingleInstance();
        builder.RegisterType<PinReactHandler>().As<IReactionHandler>();
    }
}