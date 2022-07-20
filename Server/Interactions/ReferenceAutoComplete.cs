using Discord.Interactions;
using Server.Data;

namespace TheOracle2;

public class ReferenceAutoComplete : AutocompleteHandler
{
    public IMoveRepository? Moves { get; set; }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();
            var userText = autocompleteInteraction.Data.Current.Value as string;
            var userId = autocompleteInteraction.User.Id;
            var guildId = context.Guild?.Id ?? autocompleteInteraction.User.Id;

            if (Moves == null) return Task.FromResult(AutocompletionResult.FromSuccess(successList));

            if (userText?.Length > 0)
            {
                    successList = Moves.GetMoves()
                        .Where(m => m.Name.Contains(userText, StringComparison.OrdinalIgnoreCase) || m.Category.Contains(userText, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(m => m.Name)
                        .Take(SelectMenuBuilder.MaxOptionCount)
                        .Select(m => new AutocompleteResult($"{m.Name} [{m.Parent?.Name ?? m.Category}]", m.Id.ToString())).AsEnumerable();
            }
            else
            {
                var initialMoves = new List<string> { "Face Danger", "Secure an Advantage" };
                successList = Moves.GetMoves().Where(m => initialMoves.Any(im => m.Name.Contains(im, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(m => m.Name)
                    .Take(SelectMenuBuilder.MaxOptionCount)
                    .Select(m => new AutocompleteResult($"{m.Name} [{m.Parent?.Name ?? m.Category}]", m.Id.ToString())).AsEnumerable();
            }

            return Task.FromResult(AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }
}
