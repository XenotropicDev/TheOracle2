using System.Diagnostics;
using System.Text.RegularExpressions;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2;

public class ImpactAutocomplete : AutocompleteHandler
{
    public ILogger<ImpactAutocomplete> logger { get; set; }

    // might make sense to put this in a DB, but there's not that many to keep track of anyways v0v
    public readonly List<AutocompleteResult> PlayerImpacts =
        Impacts
            .Where(impact =>
                impact.Value.AppliesTo.Contains("PC"))
                .Select(impact =>
                    new AutocompleteResult(impact.Key, impact.Value.Name)).ToList();
    // is there a way to check against other autocomplete fields in the same slash command?
    // cuz then we could filter for ones the PC already has
    public static Dictionary<string, Term> Impacts
    {
        get
        {
            var dictionary = new Dictionary<string, Term>();
            var baseDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Data"));
            var file = baseDir.GetFiles("glossary.json").FirstOrDefault();
            string text = file.OpenText().ReadToEnd();
            var glossary = JsonConvert.DeserializeObject<List<GlossaryRoot>>(text);
            var impacts = glossary.Find(item => item.Name == "Impact").Terms.ToList();
            impacts.ForEach(item => dictionary.Add(item.Name, item));
            return dictionary;
        }
    }
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var userText = autocompleteInteraction.Data.Current.Value as string;
        try
        {
            if (string.IsNullOrEmpty(userText))
            {
                return Task.FromResult(AutocompletionResult.FromSuccess(PlayerImpacts));
            }
            return Task.FromResult(
                AutocompletionResult.FromSuccess(
                    PlayerImpacts
                        .Where(impact =>
                            impact.Name.StartsWith(userText, ignoreCase: true, null)
                        )));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }
}
