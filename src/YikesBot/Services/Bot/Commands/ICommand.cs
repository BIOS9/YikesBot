using Discord;

namespace YikesBot.Services.Bot;

public interface ICommand
{
    string Name { get; }
    SlashCommandProperties Build();
    Task ExecuteAsync(ISlashCommandInteraction command);
}