using Discord;
using YikesBot.Services.Bot;

namespace YikesBot.Services.Purge;

public class PurgeCommand : ICommand
{
    public string Name => "purge";
    public SlashCommandProperties Build()
    {
        return new SlashCommandBuilder()
            .WithName("purge")
            .WithDescription("Deletes one or more messages in the current channel.")
            .WithDefaultMemberPermissions(GuildPermission.Administrator)
            .AddOption(
                "number",
                ApplicationCommandOptionType.Integer,
                "The maximum number of messages to delete.",
                true,
                minValue: 1,
                maxValue: 500)
            .AddOption(
                "user",
                ApplicationCommandOptionType.User,
                "Only delete messages from this user.",
                false)
            .Build();
    }

    public Task ExecuteAsync(ISlashCommandInteraction command)
    {
        throw new NotImplementedException();
    }
}