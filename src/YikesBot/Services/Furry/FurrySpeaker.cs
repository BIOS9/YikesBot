﻿using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using YikesBot.Services.Bot;

namespace YikesBot.Services.Furry;

public class FurrySpeaker
{
    private readonly ILogger<FurrySpeaker> _logger;
    private readonly DiscordBot _discordBot;
    
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
    
    private Task DiscordClientOnMessageReceived(SocketMessage arg)
    {
        throw new NotImplementedException();
    }
}