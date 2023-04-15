using System.Collections.Concurrent;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YikesBot.Services.Bot;
using YikesBot.Services.DeletedMessages;

namespace YikesBot.Services.ModerationLog;

public class ModerationLogger : IHostedService
{
    private const string LogChannelName = "moderation-log";

    private readonly ILogger<ModerationLogger> _logger;
    private readonly DiscordBot _discordBot;

    public ModerationLogger(ILogger<ModerationLogger> logger, DiscordBot discordBot)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _discordBot.DiscordClient.UserJoined += DiscordClientOnUserJoined;
        _discordBot.DiscordClient.UserLeft += DiscordClientOnUserLeft;
        _discordBot.DiscordClient.GuildAvailable += CreateLogChannelAsync;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _discordBot.DiscordClient.UserJoined -= DiscordClientOnUserJoined;
        _discordBot.DiscordClient.UserLeft -= DiscordClientOnUserLeft;
        _discordBot.DiscordClient.GuildAvailable -= CreateLogChannelAsync;
        return Task.CompletedTask;
    }

    private Task DiscordClientOnUserJoined(SocketGuildUser arg)
    {
        throw new NotImplementedException();
    }
    
    private Task DiscordClientOnUserLeft(SocketGuild arg1, SocketUser arg2)
    {
        throw new NotImplementedException();
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
            _logger.LogInformation("Creating moderation log channel in guild {GuildName} {GuildID}", guild.Name,
                guild.Id);
            var permissionOverrides = new OverwritePermissions(viewChannel: PermValue.Deny);
            var logChannel = await guild.CreateTextChannelAsync(LogChannelName);
            await logChannel.AddPermissionOverwriteAsync(guild.EveryoneRole, permissionOverrides);
        }
    }
}