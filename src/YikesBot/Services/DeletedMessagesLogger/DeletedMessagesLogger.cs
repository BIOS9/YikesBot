using Discord;
using Microsoft.Extensions.Logging;

namespace YikesBot.Services.DeletedMessagesLogger;

public class DeletedMessagesLogger
{
    private readonly ILogger<DeletedMessagesLogger> _logger;
    private readonly DiscordBot.DiscordBot _discordBot;

    public DeletedMessagesLogger(ILogger<DeletedMessagesLogger> logger, DiscordBot.DiscordBot discordBot)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
        
        _discordBot.DiscordClient.MessageDeleted += DiscordClientOnMessageDeleted;
    }

    private Task DiscordClientOnMessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
    {
        throw new NotImplementedException();
    }
}