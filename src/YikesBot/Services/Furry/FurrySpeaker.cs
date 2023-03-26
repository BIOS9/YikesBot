using System.Collections.Concurrent;
using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using YikesBot.Services.Bot;
using YikesBot.Services.DeletedMessages;

namespace YikesBot.Services.Furry;

public class FurrySpeaker
{
    private readonly ILogger<FurrySpeaker> _logger;
    private readonly DiscordBot _discordBot;
    private readonly DeletedMessagesLogger _deletedMessagesLogger;
    private readonly ConcurrentDictionary<ulong, string> _enabledChannels = new();
    public ISet<ulong> EnabledChannels => _enabledChannels.Keys.ToHashSet();
    
    public FurrySpeaker(
        ILogger<FurrySpeaker> logger,
        DiscordBot discordBot,
        DeletedMessagesLogger deletedMessagesLogger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
        _deletedMessagesLogger = deletedMessagesLogger ?? throw new ArgumentNullException(nameof(deletedMessagesLogger));
    }

    public void Start()
    {
        _discordBot.DiscordClient.MessageReceived += DiscordClientOnMessageReceived;
    }
    
    public void Stop()
    {
        _discordBot.DiscordClient.MessageReceived -= DiscordClientOnMessageReceived;
    }

    public void EnableChannel(SocketGuildChannel channel, string webhook)
    {
        if (channel == null) throw new ArgumentNullException(nameof(channel));
        _logger.LogInformation("Enabling furry speaker in {Guild}/{Channel}", channel.Guild, channel.Name);
        _ = _enabledChannels.TryAdd(channel.Id, webhook);
    }
    
    public void DisableChannel(SocketGuildChannel channel)
    {
        if (channel == null) throw new ArgumentNullException(nameof(channel));
        _logger.LogInformation("Disabling furry speaker in {Guild}/{Channel}", channel.Guild, channel.Name);
        _ = _enabledChannels.TryRemove(channel.Id, out _);
    }

    public void DisableAllChannels(SocketGuild guild)
    {
        _logger.LogInformation("Disabling all furry speak channels in guild {Guild}", guild.Name);
        foreach (var channel in guild.TextChannels)
        {
            _ = _enabledChannels.TryRemove(channel.Id, out _);
        }
    }
    
    private async Task DiscordClientOnMessageReceived(SocketMessage message)
    {
        if (message.Author.IsBot || message.Author.Id == _discordBot.DiscordClient.CurrentUser.Id) return;
        if (!_enabledChannels.ContainsKey(message.Channel.Id)) return;
        
        _deletedMessagesLogger.IgnoreMessage(message.Id);
        await message.DeleteAsync();

        DiscordWebhookClient client = new DiscordWebhookClient(_enabledChannels[message.Channel.Id]);


        Dictionary<string, FileAttachment> newAttachments = new();
        foreach (var attachment in message.Attachments)
        {
            if (!attachment.ContentType.StartsWith("image")) continue;

            string path = Path.GetTempFileName();
            using (var httpClient = new HttpClient())
            await using (var s = await httpClient.GetStreamAsync(attachment.Url))
            await using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                await s.CopyToAsync(fs);
            
            newAttachments.Add(path, new FileAttachment(path, attachment.Filename, attachment.Description, attachment.IsSpoiler()));
        }

        if (newAttachments.Any())
        {
            await client.SendFilesAsync(newAttachments.Values, FurryTranslator.Translate(message.Content), username: message.Author.Username,
                avatarUrl: message.Author.GetAvatarUrl(), allowedMentions: AllowedMentions.None);   
        }
        else
        {
            await client.SendMessageAsync(FurryTranslator.Translate(message.Content), username: message.Author.Username,
                avatarUrl: message.Author.GetAvatarUrl(), allowedMentions: AllowedMentions.None);   
        }

        foreach (var attachment in newAttachments)
        {
            File.Delete(attachment.Key);
        }
    }
}