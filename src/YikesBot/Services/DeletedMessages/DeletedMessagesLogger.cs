using System.Collections.Concurrent;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YikesBot.Services.Bot;

namespace YikesBot.Services.DeletedMessages;

public class DeletedMessagesLogger : IHostedService
{
    private const string LogChannelName = "deleted-messages";

    private readonly ILogger<DeletedMessagesLogger> _logger;
    private readonly DiscordBot _discordBot;
    private readonly ConcurrentDictionary<ulong, byte> _ignoredMessages = new(); // Just using this as a hash set since C# doesnt have a concurrent one yet

    public DeletedMessagesLogger(ILogger<DeletedMessagesLogger> logger, DiscordBot discordBot)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _discordBot.DiscordClient.MessageDeleted += DiscordClientOnMessageDeleted;
        _discordBot.DiscordClient.GuildAvailable += CreateLogChannelAsync;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _ignoredMessages.Clear();
        _discordBot.DiscordClient.MessageDeleted -= DiscordClientOnMessageDeleted;
        _discordBot.DiscordClient.GuildAvailable -= CreateLogChannelAsync;
        return Task.CompletedTask;
    }

    public void IgnoreMessage(ulong messageId)
    {
        _logger.LogDebug("Ignoring deleted message with ID {Id}", messageId);
        _ = _ignoredMessages.TryAdd(messageId, 0);
    }
    
    private async Task DiscordClientOnMessageDeleted(Cacheable<IMessage, ulong> cachableMessage,
        Cacheable<IMessageChannel, ulong> cachableChannel)
    {
        if (_ignoredMessages.TryRemove(cachableMessage.Id, out _))
        {
            return;
        }
        
        if (await cachableChannel.GetOrDownloadAsync() is SocketGuildChannel channel)
        {
            var logChannel = await GetLogChannelAsync(channel.Guild);
            var message = cachableMessage.Value;
            if (message == null) // If the message is not in our cache we cannot get the content
            {
                return;
            }

            IUser? suspectedDeleter = null;
            var auditLogs = channel.Guild.GetAuditLogsAsync(10, actionType: ActionType.MessageDeleted);
            await foreach (var logs in auditLogs)
            {
                if (suspectedDeleter != null)
                {
                    break;
                }
                
                foreach (var log in logs)
                {
                    if (!(log.Data is MessageDeleteAuditLogData messageDeleteData) ||   // Ensure audit log is for deleted message
                        messageDeleteData.ChannelId != channel.Id ||                    // Ensure channel matches deleted message
                        messageDeleteData.Target.Id != message.Author.Id ||             // Ensure author matches deleted message
                        log.CreatedAt < DateTimeOffset.UtcNow - TimeSpan.FromMinutes(10)) // Multiple message deletes get saved under the first timestamp   
                    {
                        continue;
                    }

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
                        x.Name = $"{message.Author.Username}#{message.Author.Discriminator}";
                        x.IconUrl = message.Author.GetAvatarUrl(ImageFormat.Auto, 256);
                    })
                    .WithFooter($"User: {message.Author.Id} • Message: {message.Id}");

                await logChannel.SendMessageAsync(string.Empty, embed: embed.Build());
            }

            foreach (var attachment in message.Attachments)
            {
                if (!attachment.ContentType.StartsWith("image"))
                {
                    continue;
                }

                _logger.LogInformation("Logging deleted image from {Author} in {Guild}/{Channel}",
                    message.Author.Username + "#" + message.Author.Discriminator,
                    channel.Guild.Name,
                    channel.Name);


                string path = Path.GetTempFileName();
                using (var client = new HttpClient())
                await using (var s = await client.GetStreamAsync(attachment.Url))
                await using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    await s.CopyToAsync(fs);
                }
                
                var embed = new EmbedBuilder()
                    .WithDescription($"**Image from {message.Author.Mention} deleted in <#{message.Channel.Id}>" +
                                     (suspectedDeleter != null
                                         ? $" probably by {suspectedDeleter.Mention}"
                                         : string.Empty) +
                                     "**\n" + message.Content)
                    .WithColor(new Color(254, 204, 80))
                    .WithCurrentTimestamp()
                    .WithImageUrl($"attachment://{attachment.Filename}")
                    .WithAuthor(x =>
                    {
                        x.Name = $"{message.Author.Username}#{message.Author.Discriminator}";
                        x.IconUrl = message.Author.GetAvatarUrl(ImageFormat.Auto, 256);
                    })
                    .WithFooter($"User: {message.Author.Id} • Message: {message.Id}");
                await logChannel.SendFileAsync(File.Open(path, FileMode.Open), embed: embed.Build(), filename: attachment.Filename);
                File.Delete(path);
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