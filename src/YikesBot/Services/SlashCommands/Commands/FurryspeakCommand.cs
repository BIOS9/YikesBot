using Discord;
using Discord.WebSocket;
using YikesBot.Services.Bot;
using YikesBot.Services.Furry;

namespace YikesBot.Services.SlashCommands.Commands;

public class FurryspeakCommand : ICommand
{
    public string Name => "furryspeak";

    private readonly DiscordBot _discordBot;
    private readonly FurrySpeaker _furrySpeaker;

    public FurryspeakCommand(DiscordBot discordBot, FurrySpeaker furrySpeaker)
    {
        _discordBot = discordBot ?? throw new ArgumentNullException(nameof(discordBot));
        _furrySpeaker = furrySpeaker ?? throw new ArgumentNullException(nameof(furrySpeaker));
    }
    
    public SlashCommandProperties Build()
    {
        return new SlashCommandBuilder()
            .WithName(Name)
            .WithDescription("Tuwns teh cuwwent channyew into a fuwwy fwiendwy zone owo!")
            .WithDefaultMemberPermissions(GuildPermission.Administrator)
            .AddOption(
                "enabled",
                ApplicationCommandOptionType.Boolean,
                "True to enable furry speak, false to disable.",
                true)
            .Build();
    }

    public async Task ExecuteAsync(ISlashCommandInteraction command)
    {
        if (command.ChannelId == null)
        {
            await command.RespondAsync("This command can only be used in a guild channel.", ephemeral: true);
            return;
        }

        IChannel channel = await _discordBot.DiscordClient.GetChannelAsync(command.ChannelId ?? 0);
        if (channel is not SocketGuildChannel textChannel)
        {
            await command.RespondAsync("This command can only be used in a text channel.", ephemeral: true);
            return;
        }

        bool enabled = (bool)command.Data.Options.First().Value;
        if (enabled)
        {
            _furrySpeaker.EnableChannel(textChannel);
            await command.RespondAsync("OwO wats dis? Furry speak has just been enabwed in dis channew~ rawr xD UwU *pees*", ephemeral: true);
        }
        else
        {
            _furrySpeaker.DisableChannel(textChannel);
            await command.RespondAsync("Furry speak has been disabled in this channel :(", ephemeral: true);
        }
    }
}