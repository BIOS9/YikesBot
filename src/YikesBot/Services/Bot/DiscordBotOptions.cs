using System.ComponentModel.DataAnnotations;

namespace YikesBot.Services.Bot;

public class DiscordBotOptions
{
    public const string Name = "DiscordBot";
    
    [Required]
    public string Token { get; init; }
}