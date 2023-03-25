using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace YikesBot.Services.DiscordBot;

public class DiscordBot : IHostedService
{
    private readonly ILogger<DiscordBot> _logger;
    private readonly ILogger<DiscordSocketClient> _botLogger;
    private readonly DiscordBotOptions _options;
    private readonly DiscordSocketClient _client;

    private readonly IReadOnlyDictionary<LogSeverity, LogLevel>
        _logLevelMap = // Maps Discord.NET logging levels to Microsoft extensions logging levels.
            new Dictionary<LogSeverity, LogLevel>
            {
                { LogSeverity.Debug, LogLevel.Trace },
                { LogSeverity.Verbose, LogLevel.Debug },
                { LogSeverity.Info, LogLevel.Information },
                { LogSeverity.Warning, LogLevel.Warning },
                { LogSeverity.Error, LogLevel.Error },
                { LogSeverity.Critical, LogLevel.Critical }
            };

    public DiscordBot(ILoggerFactory loggerFactory, IOptions<DiscordBotOptions> options)
    {
        if(loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
        _logger = loggerFactory.CreateLogger<DiscordBot>();
        _botLogger = loggerFactory.CreateLogger<DiscordSocketClient>();
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Verbose,
            LogGatewayIntentWarnings = true,
            AlwaysDownloadUsers = true,
            MessageCacheSize = 200,
            GatewayIntents = GatewayIntents.AllUnprivileged
        });
        
        _client.Log += ClientOnLog;
    }

    private Task ClientOnLog(LogMessage arg)
    {
        LogLevel level = _logLevelMap[arg.Severity];
        _botLogger.Log(level, arg.Exception, "{source} {message}", arg.Source, arg.Message);
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Discord bot starting");
        await _client.LoginAsync(TokenType.Bot, _options.Token);
        await _client.StartAsync();
        _logger.LogInformation("Discord bot running");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync();
    }
}