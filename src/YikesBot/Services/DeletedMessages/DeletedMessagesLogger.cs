using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using YikesBot.Services.Bot;

namespace YikesBot.Services.DeletedMessages;

public class DeletedMessagesLogger
{
    private const string LogChannelName = "deleted-messages";

    private readonly ILogger<DeletedMessagesLogger> _logger;
    private readonly DiscordBot _discordBot;
    
    public DeletedMessagesLogger(ILogger<DeletedMessagesLogger> logger, DiscordBot discordBot)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
        
        _discordBot.DiscordClient.MessageDeleted += DiscordClientOnMessageDeleted;
        _discordBot.DiscordClient.GuildAvailable += CreateLogChannelAsync;
    }

    private async Task DiscordClientOnMessageDeleted(Cacheable<IMessage, ulong> cachableMessage, Cacheable<IMessageChannel, ulong> cachableChannel)
    {
        if (await cachableChannel.GetOrDownloadAsync() is SocketGuildChannel channel)
        {
            IMessageChannel logChannel = await GetLogChannelAsync(channel.Guild);
            IMessage message = await cachableMessage.GetOrDownloadAsync();
            if (message == null) return; // If the message is not in our cache we cannot get the content
            await logChannel.SendMessageAsync(message.Content);
        }
    }

    private async Task<IMessageChannel> GetLogChannelAsync(IGuild guild)
    {
        var channels = await guild.GetTextChannelsAsync(CacheMode.AllowDownload);
        return channels.First(x => x.Name.Equals(LogChannelName));
    }

    private async Task CreateLogChannelAsync(IGuild guild)
    {
        var channels = await guild.GetTextChannelsAsync(CacheMode.AllowDownload);
        if (!channels.Any(x => x.Name.Equals(LogChannelName)))
        {
            _logger.LogInformation("Creating deleted messages channel in guild {GuildName} {GuildID}", guild.Name, guild.Id);
            var permissionOverrides = new OverwritePermissions(viewChannel:PermValue.Deny);
            var logChannel = await guild.CreateTextChannelAsync(LogChannelName);
            await logChannel.AddPermissionOverwriteAsync(guild.EveryoneRole, permissionOverrides);
        }
    }
}