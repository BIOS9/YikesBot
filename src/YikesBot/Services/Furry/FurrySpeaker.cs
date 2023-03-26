using System.Collections.Concurrent;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using YikesBot.Services.Bot;

namespace YikesBot.Services.Furry;

public class FurrySpeaker
{
    private readonly ILogger<FurrySpeaker> _logger;
    private readonly DiscordBot _discordBot;
    private readonly ConcurrentDictionary<ulong, SocketGuildChannel> _enabledChannels = new();
    public ISet<SocketGuildChannel> EnabledChannels => _enabledChannels.Values.ToHashSet();
    
    public FurrySpeaker(ILogger<FurrySpeaker> logger, DiscordBot discordBot)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
    }

    public void Start()
    {
        _discordBot.DiscordClient.MessageReceived += DiscordClientOnMessageReceived;
    }
    
    public void Stop()
    {
        _discordBot.DiscordClient.MessageReceived -= DiscordClientOnMessageReceived;
    }

    public void EnableChannel(SocketGuildChannel channel)
    {
        if (channel == null) throw new ArgumentNullException(nameof(channel));
        _logger.LogInformation("Enabling furry speaker in {Guild}/{Channel}", channel.Guild, channel.Name);
        _ = _enabledChannels.TryAdd(channel.Id, channel);
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
    
    private Task DiscordClientOnMessageReceived(SocketMessage arg)
    {
        
    }
}