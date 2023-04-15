using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using YikesBot.Services.Bot;
using YikesBot.Services.DeletedMessages;
using YikesBot.Services.Furry;
using YikesBot.Services.MessageContent;
using YikesBot.Services.MessageReactions;
using YikesBot.Services.ModerationLog;
using YikesBot.Services.SlashCommands;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddEnvironmentVariables();
        config.AddUserSecrets<Program>();
    })
    .UseSerilog((context, config) => { config.ReadFrom.Configuration(context.Configuration); })
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>((context, builder) =>
    {
        builder.RegisterModule(new BotModule(context.Configuration));
        builder.RegisterModule(new SlashCommandsModule());
        builder.RegisterModule(new FurryModule());
        builder.RegisterModule(new MessageContentModule());
        builder.RegisterModule(new MessageReactionsModule());
        builder.RegisterModule(new DeletedMessagesModule());
        builder.RegisterModule(new ModerationLogModule());
    })
    .Build()
    .RunAsync();