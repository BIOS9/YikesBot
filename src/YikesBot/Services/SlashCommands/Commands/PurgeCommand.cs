using Discord;
using Discord.WebSocket;
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
                "limit",
                ApplicationCommandOptionType.Integer,
                "The maximum number of messages to delete.",
                true,
                minValue: 1,
                maxValue: 1000)
            .AddOption(
                "user",
                ApplicationCommandOptionType.User,
                "Only delete messages from this user.",
                false)
            .Build();
    }

    public async Task ExecuteAsync(ISlashCommandInteraction command)
    {
        if (command.ChannelId == null)
        {
            await command.FollowupAsync("This command can only be used in a guild channel.", ephemeral: true);
            return;
        }
        
        if (!(await _discordBot.DiscordClient.GetChannelAsync(command.ChannelId ?? 0) is IMessageChannel channel))
        {
            await command.FollowupAsync("This command can only be used in a text channel.", ephemeral: true);
            return;
        }

        if (command.Data.Options.Count == 1) // If only the number is specified
        {
            int count = (int)(long)command.Data.Options.First().Value;
            IEnumerable<IMessage> messages = await channel.GetMessagesAsync(count).FlattenAsync();
            messages = messages
                .Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14); // Cant use bulk delete API on messages older than 14 days.
            await ((ITextChannel)channel).DeleteMessagesAsync(messages);
            await command.FollowupAsync("Messages deleted.", ephemeral: true);
        } 
        else if (command.Data.Options.Count == 2)
        {
            int count = (int)(long)command.Data.Options.First().Value;
            SocketUser user = (SocketUser)command.Data.Options.Skip(1).First().Value;
            // This is pretty bad, it will not actually delete count messages if there are lots of other messages between. * 2 is a hopeful mitigation.
            IEnumerable<IMessage> messages = await channel.GetMessagesAsync(Math.Max(count * 2, 50)).FlattenAsync();
            messages = messages
                .Where(x => x.Author.Id == user.Id)
                .Take(count)
                .Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);
            await ((ITextChannel)channel).DeleteMessagesAsync(messages);
            await command.FollowupAsync("Messages deleted.", ephemeral: true);
        }
        else
        {
            await command.FollowupAsync("Invalid number of arguments.", ephemeral: true);
        }
    }
}