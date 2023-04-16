using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YikesBot.Services.Bot;

namespace YikesBot.Services.MessageContent;

public class MessageContentHandler : IHostedService
{
    private readonly ILogger<MessageContentHandler> _logger;
    private readonly DiscordBot _discordBot;
    private readonly IEnumerable<IContentHandler> _contentHandlers;
    
    public MessageContentHandler(
        ILogger<MessageContentHandler> logger,
        DiscordBot discordBot,
        IEnumerable<IContentHandler> contentHandlers)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
        _contentHandlers = contentHandlers ?? throw new ArgumentNullException(nameof(contentHandlers));
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

    private async Task DiscordClientOnMessageReceived(SocketMessage message)
    {
        if (message.Author.IsBot || message.Author.Id == _discordBot.DiscordClient.CurrentUser.Id)
        {
            return;
        }

        foreach (var handler in _contentHandlers)
        {
            if (await handler.ExecuteAsync(message))
            {
                _logger.LogInformation("Executed message content handler: {Handler}", handler.Name);
                return;
            }
        }
    }
}