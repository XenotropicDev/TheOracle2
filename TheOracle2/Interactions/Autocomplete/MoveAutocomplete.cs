using Discord.Interactions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2.Commands;

public class MoveAutocomplete : AutocompleteHandler
{
    private static readonly Dictionary<string, Task<AutocompletionResult>> dict = new Dictionary<string, Task<AutocompletionResult>>();

    private Task<AutocompletionResult> initialMoveResults
    {
        get
        {
            if (dict.TryGetValue("initialMoves", out Task<AutocompletionResult> result)) return result;

            var moves = Db.Moves.Where(o => o.Name == "Face Danger" || o.Name == "Secure an Advantage" || o.Name == "Pay the Price" || o.Name == "Take a Break" || o.Name == "Change Your Fate")
                .AsEnumerable();
            var list = moves
                .Select(x => new AutocompleteResult(x.Name, x.Id))
                .OrderBy(x => //Todo this is really lazy ordering, but so is the rest of this getter's code.
                    x.Name == "Face Danger" ? 1 :
                    x.Name == "Secure an Advantage" ? 2 :
                    x.Name == "Pay the Price" ? 3 :
                    x.Name == "Take a Break" ? 4 : 5
                    )
                .Take(SelectMenuBuilder.MaxOptionCount);

            result = Task.FromResult(AutocompletionResult.FromSuccess(list));

            dict.Add("initialMoves", result);

            return result;
        }
    }

    public EFContext Db { get; set; }
    public ILogger<MoveAutocomplete> logger { get; set; }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();

            var value = autocompleteInteraction.Data.Current.Value as string;

            if (string.IsNullOrEmpty(value))
            {
                return initialMoveResults;
            }

            var moves = Db.Moves.Where(x => Regex.IsMatch(x.Name, $@"\b(?i){value}"));
            successList = moves.Select(x => new AutocompleteResult(x.Name, x.Id.ToString())).Take(SelectMenuBuilder.MaxOptionCount);

            return Task.FromResult(AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }

    protected override string GetLogString(IInteractionContext context) => $"Accessing DB from {context.Guild}-{context.Channel}";
}
