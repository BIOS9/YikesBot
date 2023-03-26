using Microsoft.Extensions.Hosting;
using YikesBot.Services.Bot;
using YikesBot.Services.DeletedMessages;
using YikesBot.Services.MessageContent;
using YikesBot.Services.SlashCommands;

namespace YikesBot;

public class Startup : IHostedService
{
    private readonly DiscordBot _discordBot;
    private readonly DeletedMessagesLogger _deletedMessagesLogger;
    private readonly MessageContentHandler _messageContentHandler;
    private readonly SlashCommandHandler _slashCommandHandler;

    public Startup(
        DiscordBot discordBot,
        DeletedMessagesLogger deletedMessagesLogger,
        MessageContentHandler messageContentHandler,
        SlashCommandHandler slashCommandHandler)
    {
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
        _deletedMessagesLogger =
            deletedMessagesLogger ?? throw new ArgumentNullException(nameof(deletedMessagesLogger));
        _messageContentHandler =
            messageContentHandler ?? throw new ArgumentNullException(nameof(messageContentHandler));
        _slashCommandHandler =
            slashCommandHandler ?? throw new ArgumentNullException(nameof(slashCommandHandler));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _discordBot.StartAsync(cancellationToken);
        _deletedMessagesLogger.Start();
        _messageContentHandler.Start();
        _slashCommandHandler.Start();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _slashCommandHandler.Stop();
        _messageContentHandler.Stop();
        _deletedMessagesLogger.Stop();
        await _discordBot.StartAsync(cancellationToken);
    }
}