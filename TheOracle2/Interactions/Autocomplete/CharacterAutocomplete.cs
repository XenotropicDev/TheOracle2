using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using TheOracle2.UserContent;

namespace TheOracle2;

public class CharacterAutocomplete : AutocompleteHandler
{
    public EFContext Db { get; set; }
    private const int BroadenSearchAt = 4;

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();
            var userText = autocompleteInteraction.Data.Current.Value as string;
            var userId = autocompleteInteraction.User.Id;
            var guildId = context.Guild?.Id ?? autocompleteInteraction.User.Id;

            switch (userText.Length)
            {
                case > 0 and < BroadenSearchAt:
                    {
                        // return list of guild PCs that start with query; own PCs at top.
                        successList = Db.PlayerCharacters
                            .Where((pc) => pc.DiscordGuildId == guildId && pc.Name.StartsWith(userText))
                            .OrderBy(pc => pc.UserId != pc.UserId).ThenBy(pc => pc.Name)
                            .Take(SelectMenuBuilder.MaxOptionCount)
                            .Select(pc => new AutocompleteResult(pc.Name, pc.Id.ToString())).AsEnumerable();
                    }
                    break;
                case >= BroadenSearchAt:
                    {
                        // if the user still hasn't found the character, broaden search to "Contains". fuzzier matching might be better tho
                        successList = Db.PlayerCharacters
                            .Where((pc) => pc.DiscordGuildId == guildId && pc.Name.Contains(userText))
                            .OrderBy(pc => pc.UserId != pc.UserId).ThenBy(pc => pc.Name)
                            .Take(SelectMenuBuilder.MaxOptionCount)
                            .Select(pc => new AutocompleteResult(pc.Name, pc.Id.ToString())).AsEnumerable();
                    }
                    break;
                default:
                    {
                        // fallback to list of users own guild PCs, sorted alphabetically
                        successList = Db.PlayerCharacters
                            .Where((pc) => pc.DiscordGuildId == guildId && pc.UserId == userId)
                            .OrderBy(pc => pc.Name)
                            .Take(SelectMenuBuilder.MaxOptionCount)
                            .Select(pc => new AutocompleteResult(pc.Name, pc.Id.ToString())).AsEnumerable();
                    }
                    break;
            }
            return Task.FromResult(AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }
}
