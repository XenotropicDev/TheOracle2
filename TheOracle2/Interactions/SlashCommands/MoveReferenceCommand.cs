using Discord.Interactions;
using TheOracle2.Commands;
using TheOracle2.UserContent;

namespace TheOracle2;

public class MoveReferenceCommand : InteractionModuleBase
{
    public MoveReferenceCommand(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public EFContext DbContext { get; }

    [SlashCommand("move", "Posts the game text for a move. To roll a move, use /roll.")]
    public async Task PostAsset([Autocomplete(typeof(MoveAutocomplete))] string move, bool ephemeral = false, bool keepMessage = false)
    {
        var movedata = DbContext.Moves.Find(move);
        var entityItem = new DiscordMoveEntity(movedata);

        await RespondAsync(embeds: entityItem.GetEmbeds(), ephemeral: ephemeral || entityItem.IsEphemeral, components: entityItem.GetComponents());

        if (!keepMessage && !ephemeral)
        {
            await Task.Delay(TimeSpan.FromMinutes(10)).ConfigureAwait(false);
            await DeleteOriginalResponseAsync();
        }
    }
}
