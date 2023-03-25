using Microsoft.Extensions.Hosting;
using YikesBot.Services.Bot;
using YikesBot.Services.DeletedMessages;

namespace YikesBot;

public class Startup : IHostedService
{
    private readonly DiscordBot _discordBot;
    private readonly DeletedMessagesLogger _deletedMessagesLogger;

    public Startup(DiscordBot discordBot, DeletedMessagesLogger deletedMessagesLogger)
    {
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
        _deletedMessagesLogger =
            deletedMessagesLogger ?? throw new ArgumentNullException(nameof(deletedMessagesLogger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _discordBot.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordBot.StartAsync(cancellationToken);
    }
}