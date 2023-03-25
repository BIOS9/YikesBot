using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace YikesBot.Services.DiscordBot;

public class DiscordBot : IHostedService
{
    private readonly ILogger<DiscordBot> _logger;
    private readonly DiscordBotOptions _options;

    public DiscordBot(ILogger<DiscordBot> logger, IOptions<DiscordBotOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}