using Discord;
using Discord.WebSocket;

namespace YikesBot.Services.MessageReactions;

public interface IReactionHandler
{
    string Name { get; }
    Task<bool> ExecuteAsync(IUserMessage message, IMessageChannel channel, SocketReaction reaction);
}