using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace YikesBot.Services.MessageContent.ContentHandlers;

public class AwooHandler : IContentHandler
{
    public string Name => "Awoo";

    private const string AwooEmoji = "<:awooo:958999403975290970>";
    private static readonly Regex Pattern = new ("^awo{2,}$", RegexOptions.IgnoreCase);
    
    public Task<bool> IsMatchAsync(SocketMessage message)
    {
        bool match =
            message.Content.Equals(AwooEmoji) ||
            Pattern.IsMatch(message.Content);
        return Task.FromResult(match);
    }

    public async Task<bool> ExecuteAsync(SocketMessage message)
    {
        if (!await IsMatchAsync(message))
        {
            return false;
        }
        await message.Channel.SendMessageAsync(AwooEmoji);
        return true;
    }
}