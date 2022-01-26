using System.Text.RegularExpressions;
using Discord.Interactions;
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
                            // '\b' instead of '^' to handle cases like searching 'Izar' for 'Celebrant Izar'
                            .Where((pc) => pc.DiscordGuildId == guildId && Regex.IsMatch(pc.Name, $@"\b(?i){userText}"))
                            // TODO: write a custom sort method
                            // not-equal-to operator below is intentional: 'false' (0) comes before 'true' (1) in sorting
                            .OrderBy(pc => pc.UserId != userId).ThenBy(pc => pc.Name)
                            .Take(SelectMenuBuilder.MaxOptionCount)
                            .Select(pc => new AutocompleteResult(pc.Name, pc.Id.ToString())).AsEnumerable();
                    }
                    break;

                case >= BroadenSearchAt:
                    {
                        // if the user still hasn't found the character, broaden search to strings within words
                        successList = Db.PlayerCharacters
                            .Where((pc) => pc.DiscordGuildId == guildId && Regex.IsMatch(pc.Name, $"(?i){userText}"))
                            .OrderBy(pc => pc.UserId != userId).ThenBy(pc => pc.Name)
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
