using Discord.Interactions;
using TheOracle2.UserContent;

namespace TheOracle2;

[Group("edit-player", "Edits player stats")]
public class EditPlayerPaths : InteractionModuleBase
{
    public EFContext DbContext { get; }

    public EditPlayerPaths(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    [SlashCommand("add-impact", "Changes the impacts on a character")]
    public async Task SetImpacts([Autocomplete(typeof(CharacterAutocomplete))] string character, string impact)
    {
        if (!int.TryParse(character, out var id)) return;
        var pc = await DbContext.PlayerCharacters.FindAsync(id);

        pc.Impacts.Add(impact);
        await DbContext.SaveChangesAsync();

        await RespondAsync($"Impacts will update next time you trigger an interaction on that character card", ephemeral:true);

        //await pc.RedrawCard(Context.Client);
    }

    [SlashCommand("earned-xp", "Changes the earned xp on a character")]
    public async Task SetEarnedXp([Autocomplete(typeof(CharacterAutocomplete))] string character,
                                [Summary(description: "The total amount of Xp this character has earned")][MinValue(0)] int earnedXp)
    {
    }
}
