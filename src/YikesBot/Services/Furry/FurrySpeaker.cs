﻿using System.Collections.Concurrent;
using Discord;
using Discord.Rest;
using Discord.Webhook;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YikesBot.Services.Bot;
using YikesBot.Services.DeletedMessages;

namespace YikesBot.Services.Furry;

public class FurrySpeaker : IHostedService
{
    private readonly ILogger<FurrySpeaker> _logger;
    private readonly DiscordBot _discordBot;
    private readonly DeletedMessagesLogger _deletedMessagesLogger;
    private readonly ConcurrentDictionary<ulong, DiscordWebhookClient> _enabledChannels = new();
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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _discordBot.DiscordClient.MessageReceived += DiscordClientOnMessageReceived;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _discordBot.DiscordClient.MessageReceived -= DiscordClientOnMessageReceived;
        return Task.CompletedTask;
    }

    public async Task EnableChannelAsync(SocketTextChannel channel)
    {
        if (channel == null)
        {
            throw new ArgumentNullException(nameof(channel));
        }
        _logger.LogInformation("Enabling furry speaker in {Guild}/{Channel}", channel.Guild, channel.Name);

        RestWebhook webhook = (await channel.GetWebhooksAsync()).FirstOrDefault(defaultValue: null)
                               ?? await channel.CreateWebhookAsync("Awooo");

        _ = _enabledChannels.TryAdd(channel.Id, new DiscordWebhookClient(webhook.Id, webhook.Token));
    }
    
    public void DisableChannel(SocketTextChannel channel)
    {
        if (channel == null)
        {
            throw new ArgumentNullException(nameof(channel));
        }
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
        if (message.Author.IsBot || message.Author.Id == _discordBot.DiscordClient.CurrentUser.Id)
        {
            return;
        }

        if (!_enabledChannels.ContainsKey(message.Channel.Id))
        {
            return;
        }
        
        // Check if a user posted the message
        if (!(message is SocketUserMessage msg))
        {
            return;
        }

        // Check if it is not a DM
        if (!(message.Author is SocketGuildUser author))
        {
            return;
        }

        _deletedMessagesLogger.IgnoreMessage(message.Id);
        await message.DeleteAsync();

        DiscordWebhookClient client = _enabledChannels[message.Channel.Id];

        Dictionary<string, FileAttachment> newAttachments = new();
        foreach (var attachment in message.Attachments)
        {
            if (!attachment.ContentType.StartsWith("image"))
            {
                continue;
            }

            string path = Path.GetTempFileName();
            using (var httpClient = new HttpClient())
            await using (var s = await httpClient.GetStreamAsync(attachment.Url))
            await using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                await s.CopyToAsync(fs);
            
            newAttachments.Add(path, new FileAttachment(path, attachment.Filename, attachment.Description, attachment.IsSpoiler()));
        }

        if (newAttachments.Any())
        {
            await client.SendFilesAsync(newAttachments.Values, FurryTranslator.Translate(message.Content),
                username: string.IsNullOrEmpty(author.Nickname) ? author.Username : author.Nickname,
                avatarUrl: message.Author.GetAvatarUrl(), allowedMentions: AllowedMentions.None);   
        }
        else
        {
            await client.SendMessageAsync(FurryTranslator.Translate(message.Content),
                username: string.IsNullOrEmpty(author.Nickname) ? author.Username : author.Nickname,
                avatarUrl: message.Author.GetAvatarUrl(), allowedMentions: AllowedMentions.None);   
        }

        foreach (var attachment in newAttachments)
        {
            File.Delete(attachment.Key);
        }
    }
}