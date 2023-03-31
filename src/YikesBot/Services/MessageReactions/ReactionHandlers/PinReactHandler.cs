using Discord;
using Discord.WebSocket;

namespace YikesBot.Services.MessageReactions.ReactionHandlers;

public class PinReactHandler : IReactionHandler
{
    public string Name => "Pin";

    private const int RequiredPinCount = 3;
    
    public async Task<bool> ExecuteAsync(IUserMessage message, IMessageChannel channel, SocketReaction reaction)
    {
        if (!reaction.Emote.Name.Equals("📌")) return false;
        var emotes = await message.GetReactionUsersAsync(new Emoji("📌"), RequiredPinCount).FlattenAsync();
        if (emotes.Count() >= RequiredPinCount && !message.IsPinned)
        {
            await message.PinAsync();
        }

        return true;
    }
}