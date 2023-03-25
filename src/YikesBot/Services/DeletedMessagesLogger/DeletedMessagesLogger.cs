using Discord;
using Microsoft.Extensions.Logging;

namespace YikesBot.Services.DeletedMessagesLogger;

public class DeletedMessagesLogger
{
    private readonly ILogger<DeletedMessagesLogger> _logger;
    private readonly DiscordBot.DiscordBot _discordBot;

    public DeletedMessagesLogger(ILogger<DeletedMessagesLogger> logger, DiscordBot.DiscordBot discordBot)
    {
        _logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
        _discordBot.DiscordClient.MessageDeleted += DiscordClientOnMessageDeleted;
    }

    private Task DiscordClientOnMessageDeleted(Cacheable<IMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2)
    {
        throw new NotImplementedException();
    }
}