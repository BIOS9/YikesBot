using Autofac;
using YikesBot.Services.SlashCommands.Commands;

namespace YikesBot.Services.SlashCommands;

public class SlashCommandsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<SlashCommandHandler>().SingleInstance();
        builder.RegisterType<PurgeCommand>().As<ICommand>();
        builder.RegisterType<MicropadCommand>().As<ICommand>();
        builder.RegisterType<FurryspeakCommand>().As<ICommand>();
    }
}