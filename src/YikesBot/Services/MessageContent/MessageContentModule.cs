using Autofac;
using YikesBot.Services.MessageContent.ContentHandlers;

namespace YikesBot.Services.MessageContent;

public class MessageContentModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MessageContentHandler>().SingleInstance();
        builder.RegisterType<AwooHandler>().As<IContentHandler>();
    }
}