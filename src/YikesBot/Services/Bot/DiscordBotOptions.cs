using System.ComponentModel.DataAnnotations;

namespace YikesBot.Services.Bot;

public class DiscordBotOptions
{
    public static string Name => "DiscordBot";

    [Required]
    public string Token { get; init; } = string.Empty;
    [StringLength(128)]
    public string StatusText { get; init; } = string.Empty;
}