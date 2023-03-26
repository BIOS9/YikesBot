using Discord;
using Discord.WebSocket;
using YikesBot.Services.Bot;

namespace YikesBot.Services.SlashCommands.Commands;

public class MicropadCommand : ICommand
{
    public string Name => "micropad";

    private static readonly string[] Screenshots =
    {
        "https://getmicropad.com/img/scrot/drawings.png",
        "https://getmicropad.com/img/scrot/midnight.png",
        "https://getmicropad.com/img/scrot/solarized.png",
        "https://getmicropad.com/img/scrot/politics-note.png",
        "https://getmicropad.com/img/scrot/main-page.png"
    };
    private static readonly Random ScreenshotRng =new();
    
    private readonly DiscordBot _discordBot;

    public MicropadCommand(DiscordBot discordBot)
    {
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
    }
    
    public SlashCommandProperties Build()
    {
        return new SlashCommandBuilder()
            .WithName("micropad")
            .WithDescription("Shills Micropad.")
            .Build();
    }

    public async Task ExecuteAsync(ISlashCommandInteraction command)
    {
        var embed = new EmbedBuilder()
            .WithTitle("μPad")
            .WithThumbnailUrl("https://raw.githubusercontent.com/MicroPad/MicroPad-Core/next-dev/app/public/launcher-icon-512.png")
            .WithImageUrl(Screenshots[ScreenshotRng.Next(Screenshots.Length)])
            .WithUrl("https://getmicropad.com/")
            .WithDescription("A powerful note-taking app that helps you organise + take notes without restrictions.")
            .WithColor(new Color(253, 178, 0))
            .WithFooter($"MicroPad: The free and open source note taking app.");
        await command.FollowupAsync(embed: embed.Build());
    }
}