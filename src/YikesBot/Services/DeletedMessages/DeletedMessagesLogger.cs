using Discord;
using Discord.Rest;
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
    }

    public void Start()
    {
        _discordBot.DiscordClient.MessageDeleted += DiscordClientOnMessageDeleted;
        _discordBot.DiscordClient.GuildAvailable += CreateLogChannelAsync;
    }
    
    public void Stop()
    {
        _discordBot.DiscordClient.MessageDeleted -= DiscordClientOnMessageDeleted;
        _discordBot.DiscordClient.GuildAvailable -= CreateLogChannelAsync;
    }

    private async Task DiscordClientOnMessageDeleted(Cacheable<IMessage, ulong> cachableMessage,
        Cacheable<IMessageChannel, ulong> cachableChannel)
    {
        if (await cachableChannel.GetOrDownloadAsync() is SocketGuildChannel channel)
        {
            var logChannel = await GetLogChannelAsync(channel.Guild);
            var message = cachableMessage.Value;
            if (message == null) return; // If the message is not in our cache we cannot get the content
            
            IUser? suspectedDeleter = null;
            var auditLogs = channel.Guild.GetAuditLogsAsync(10, actionType: ActionType.MessageDeleted);
            await foreach (var logs in auditLogs)
            {
                if (suspectedDeleter != null) break;
                foreach (var log in logs)
                {
                    if (!(log.Data is MessageDeleteAuditLogData messageDeleteData)) continue;
                    if (messageDeleteData.ChannelId != channel.Id) continue;
                    if (messageDeleteData.Target.Id != message.Author.Id) continue;
                    if (log.CreatedAt < DateTimeOffset.UtcNow - TimeSpan.FromMinutes(10))
                        continue; // Multiple message deletes get saved under the first timestamp

                    suspectedDeleter = log.User;
                    break;
                }
            }

            if (!string.IsNullOrWhiteSpace(message.Content))
            {
                _logger.LogInformation("Logging deleted message from {Author} in {Guild}/{Channel}",
                    message.Author.Username + "#" + message.Author.Discriminator,
                    channel.Guild.Name,
                    channel.Name);
                var embed = new EmbedBuilder()
                    .WithDescription($"**Message from {message.Author.Mention} deleted in <#{message.Channel.Id}> " +
                                     (suspectedDeleter != null
                                         ? $" probably by {suspectedDeleter.Mention}"
                                         : string.Empty) +
                                     "**\n" + message.Content)
                    .WithColor(new Color(254, 204, 80))
                    .WithCurrentTimestamp()
                    .WithAuthor(x =>
                    {
                        x.Name = $"{message.Author.Username}#{message.Author.DiscriminatorValue}";
                        x.IconUrl = message.Author.GetAvatarUrl(ImageFormat.Auto, 256);
                    })
                    .WithFooter($"User: {message.Author.Id} • Message: {message.Id}");
                if (suspectedDeleter != null)
                {
                }

                await logChannel.SendMessageAsync(string.Empty, embed: embed.Build());
            }

            foreach (var attachment in message.Attachments)
            {
                if (!attachment.ContentType.StartsWith("image")) continue;

                _logger.LogInformation("Logging deleted image from {Author} in {Guild}/{Channel}",
                    message.Author.Username + "#" + message.Author.Discriminator,
                    channel.Guild.Name,
                    channel.Name);

                var embed = new EmbedBuilder()
                    .WithDescription($"**Image from {message.Author.Mention} deleted in <#{message.Channel.Id}>" +
                                     (suspectedDeleter != null
                                         ? $" probably by {suspectedDeleter.Mention}"
                                         : string.Empty) +
                                     "**\n" + message.Content)
                    .WithColor(new Color(254, 204, 80))
                    .WithCurrentTimestamp()
                    .WithImageUrl(attachment.Url)
                    .WithAuthor(x =>
                    {
                        x.Name = $"{message.Author.Username}#{message.Author.DiscriminatorValue}";
                        x.IconUrl = message.Author.GetAvatarUrl(ImageFormat.Auto, 256);
                    })
                    .WithFooter($"User: {message.Author.Id} • Message: {message.Id}");
                await logChannel.SendMessageAsync(string.Empty, embed: embed.Build());
            }
        }
    }

    private async Task<IMessageChannel> GetLogChannelAsync(IGuild guild)
    {
        var channels = await guild.GetTextChannelsAsync();
        return channels.First(x => x.Name.Equals(LogChannelName));
    }

    private async Task CreateLogChannelAsync(IGuild guild)
    {
        var channels = await guild.GetTextChannelsAsync();
        if (!channels.Any(x => x.Name.Equals(LogChannelName)))
        {
            _logger.LogInformation("Creating deleted messages channel in guild {GuildName} {GuildID}", guild.Name,
                guild.Id);
            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            var logChannel = await guild.CreateTextChannelAsync(LogChannelName);
            await logChannel.AddPermissionOverwriteAsync(guild.EveryoneRole, permissionOverrides);
        }
    }
}