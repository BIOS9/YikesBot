using Discord;

namespace YikesBot.Services.SlashCommands;

public interface ICommand
{
    string Name { get; }
    SlashCommandProperties Build();
    Task ExecuteAsync(ISlashCommandInteraction command);
}