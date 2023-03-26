using Discord.WebSocket;

namespace YikesBot.Services.MessageContent;

public interface IContentHandler
{
    string Name { get; }
    Task<bool> IsMatchAsync(SocketMessage message);
    Task<bool> ExecuteAsync(SocketMessage message);
}