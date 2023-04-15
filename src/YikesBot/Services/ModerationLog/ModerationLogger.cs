using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YikesBot.Services.Bot;

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

    public void Log(string title, string description, IUser user, IGuild guild)
    {
        Log(new EmbedBuilder()
            .WithDescription($"**{title}**\n{description}".Trim())
            .WithColor(new Color(254, 204, 80))
            .WithCurrentTimestamp()
            .WithAuthor(x =>
            {
                x.Name = $"{user.Username}#{user.Discriminator}";
                x.IconUrl = user.GetAvatarUrl(ImageFormat.Auto, 256);
            })
            .WithFooter($"User: {user.Id}")
            .Build(), guild);
    }

    public async void Log(Embed embed, IGuild guild)
    {
        var channel = await GetLogChannelAsync(guild);
        await channel.SendMessageAsync(embed: embed);
    }

    private Task DiscordClientOnUserJoined(SocketGuildUser user)
    {
        Log($"Member Joined: {user.Mention}", string.Empty, user, user.Guild);
        return Task.CompletedTask;
    }
    
    private Task DiscordClientOnUserLeft(SocketGuild guild, SocketUser user)
    {
        Log($"Member Left: {user.Mention}", string.Empty, user, guild);
        return Task.CompletedTask;
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