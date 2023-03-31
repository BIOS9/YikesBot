using Microsoft.Extensions.Hosting;
using YikesBot.Services.Bot;
using YikesBot.Services.DeletedMessages;
using YikesBot.Services.Furry;
using YikesBot.Services.MessageContent;
using YikesBot.Services.MessageReactions;
using YikesBot.Services.SlashCommands;

namespace YikesBot;

public class Startup : IHostedService
{
    private readonly DiscordBot _discordBot;
    private readonly DeletedMessagesLogger _deletedMessagesLogger;
    private readonly MessageContentHandler _messageContentHandler;
    private readonly MessageReactionHandler _messageReactionHandler;
    private readonly FurrySpeaker _furrySpeaker;
    private readonly SlashCommandHandler _slashCommandHandler;

    public Startup(
        DiscordBot discordBot,
        DeletedMessagesLogger deletedMessagesLogger,
        MessageContentHandler messageContentHandler,
        MessageReactionHandler messageReactionHandler,
        SlashCommandHandler slashCommandHandler,
        FurrySpeaker furrySpeaker)
    {
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
        _furrySpeaker = furrySpeaker ?? throw new ArgumentNullException(nameof(furrySpeaker));
        _deletedMessagesLogger =
            deletedMessagesLogger ?? throw new ArgumentNullException(nameof(deletedMessagesLogger));
        _messageContentHandler =
            messageContentHandler ?? throw new ArgumentNullException(nameof(messageContentHandler));
        _messageReactionHandler =
            messageReactionHandler ?? throw new ArgumentNullException(nameof(messageReactionHandler));
        _slashCommandHandler =
            slashCommandHandler ?? throw new ArgumentNullException(nameof(slashCommandHandler));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _discordBot.StartAsync(cancellationToken);
        _deletedMessagesLogger.Start();
        _messageContentHandler.Start();
        _messageReactionHandler.Start();
        _furrySpeaker.Start();
        _slashCommandHandler.Start();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _slashCommandHandler.Stop();
        _furrySpeaker.Stop();
        _messageReactionHandler.Stop();
        _messageContentHandler.Stop();
        _deletedMessagesLogger.Stop();
        await _discordBot.StartAsync(cancellationToken);
    }
}