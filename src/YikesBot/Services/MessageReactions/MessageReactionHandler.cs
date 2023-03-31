using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using YikesBot.Services.Bot;

namespace YikesBot.Services.MessageReactions;

public class MessageReactionHandler
{
    private readonly ILogger<MessageReactionHandler> _logger;
    private readonly DiscordBot _discordBot;
    private readonly IEnumerable<IReactionHandler> _reactionHandlers;
    
    public MessageReactionHandler(
        ILogger<MessageReactionHandler> logger,
        DiscordBot discordBot,
        IEnumerable<IReactionHandler> reactionHandlers)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
        _reactionHandlers = reactionHandlers ?? throw new ArgumentNullException(nameof(reactionHandlers));
    }

    public void Start()
    {
        _discordBot.DiscordClient.ReactionAdded += DiscordClientOnReactionAdded;
    }

    public void Stop()
    {
        _discordBot.DiscordClient.ReactionAdded -= DiscordClientOnReactionAdded;
    }
    
    private async Task DiscordClientOnReactionAdded(
        Cacheable<IUserMessage, ulong> cachableMessage, 
        Cacheable<IMessageChannel, ulong> cachableChannel, 
        SocketReaction reaction)
    {
        IUserMessage message = await cachableMessage.GetOrDownloadAsync();
        IMessageChannel channel = await cachableChannel.GetOrDownloadAsync();
        
        if (message.Author.IsBot || message.Author.Id == _discordBot.DiscordClient.CurrentUser.Id)
            return;
        
        foreach (var handler in _reactionHandlers)
        {
            if (await handler.ExecuteAsync(message, channel, reaction))
            {
                _logger.LogInformation("Executed message reaction handler: {Handler}", handler.Name);
            }
        }
    }
}