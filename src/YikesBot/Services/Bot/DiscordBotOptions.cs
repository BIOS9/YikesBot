using System.ComponentModel.DataAnnotations;

namespace YikesBot.Services.Bot;

public class DiscordBotOptions
{
    public const string Name = "DiscordBot";

    [Required]
    public string Token { get; init; } = String.Empty;
    [StringLength(128)]
    public string StatusText { get; init; } = String.Empty;
}