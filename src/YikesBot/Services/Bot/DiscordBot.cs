using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace YikesBot.Services.Bot;

public class DiscordBot : IHostedService
{
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
    
    public required ILogger<DiscordSocketClient> BotLogger { protected get; init; }
    public required ILogger<DiscordBot> Logger { protected get; init; }
    public required IOptions<DiscordBotOptions> Options { protected get; init; }
    private DiscordBotOptions Config => Options.Value;
    public readonly DiscordSocketClient DiscordClient;

    public DiscordBot()
    {
        DiscordClient = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Verbose,
            LogGatewayIntentWarnings = true,
            AlwaysDownloadUsers = true,
            MessageCacheSize = 1000,
            GatewayIntents = GatewayIntents.Guilds
                             | GatewayIntents.MessageContent
                             | GatewayIntents.GuildMessages
                             | GatewayIntents.GuildMessageReactions
        });

        DiscordClient.Log += DiscordClientOnLog;
    }

    private Task DiscordClientOnLog(LogMessage arg)
    {
        var level = _logLevelMap[arg.Severity];
        BotLogger.Log(level, arg.Exception, "{source} {message}", arg.Source, arg.Message);
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Discord bot starting");
        await DiscordClient.LoginAsync(TokenType.Bot, Config.Token);
        await DiscordClient.StartAsync();
        Logger.LogInformation("Discord bot running");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await DiscordClient.StopAsync();
    }
}