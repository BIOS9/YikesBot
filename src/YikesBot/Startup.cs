using Microsoft.Extensions.Hosting;
using YikesBot.Services.Bot;
using YikesBot.Services.DeletedMessages;
using YikesBot.Services.SlashCommands;

namespace YikesBot;

public class Startup : IHostedService
{
    private readonly DeletedMessagesLogger _deletedMessagesLogger;
    private readonly DiscordBot _discordBot;
    private readonly SlashCommandHandler _slashCommandHandler;

    public Startup(
        DiscordBot discordBot, 
        DeletedMessagesLogger deletedMessagesLogger,
        SlashCommandHandler slashCommandHandler)
    {
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
        _deletedMessagesLogger =
            deletedMessagesLogger ?? throw new ArgumentNullException(nameof(deletedMessagesLogger));
        _slashCommandHandler =
            slashCommandHandler ?? throw new ArgumentNullException(nameof(slashCommandHandler));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _discordBot.StartAsync(cancellationToken);
        _deletedMessagesLogger.Start();
        _slashCommandHandler.Start();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _slashCommandHandler.Stop();
        _deletedMessagesLogger.Stop();
        await _discordBot.StartAsync(cancellationToken);
    }
}