using Autofac;
using Microsoft.Extensions.Hosting;
using YikesBot.Services.SlashCommands.Commands;

namespace YikesBot.Services.SlashCommands;

public class SlashCommandsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<SlashCommandHandler>()
            .AsSelf()
            .As<IHostedService>()
            .SingleInstance();
        builder.RegisterType<PurgeCommand>().As<ICommand>();
        builder.RegisterType<MicropadCommand>().As<ICommand>();
        builder.RegisterType<FurryspeakCommand>().As<ICommand>();
    }
}