using Discord;
using YikesBot.Services.Bot;

namespace YikesBot.Services.SlashCommands.Commands;

public class PurgeCommand : ICommand
{
    public string Name => "purge";

    private readonly DiscordBot _discordBot;

    public PurgeCommand(DiscordBot discordBot)
    {
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
    }
    
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

    public async Task ExecuteAsync(ISlashCommandInteraction command)
    {
        if (command.ChannelId != null)
        {
            await command.RespondAsync("This command can only be used in a guild channel.", ephemeral: true);
            return;
        }

        if (!(await _discordBot.DiscordClient.GetChannelAsync(command.ChannelId ?? 0) is IMessageChannel channel))
        {
            await command.RespondAsync("This command can only be used in a text channel.", ephemeral: true);
            return;
        }

        int count = (int)command.Data.Options.First().Value;
        IEnumerable<IMessage> messages = await channel.GetMessagesAsync(count + 1).FlattenAsync();
        await ((ITextChannel)channel).DeleteMessagesAsync(messages);
        await command.RespondAsync("Messages deleted.", ephemeral: true);
    }
}