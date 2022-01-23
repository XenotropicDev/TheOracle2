using System.Diagnostics;
using System.Text.RegularExpressions;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2.Commands;

public class OracleAutocomplete : AutocompleteHandler
{
    private static readonly Dictionary<string, Task<AutocompletionResult>> dict = new Dictionary<string, Task<AutocompletionResult>>();

    private Task<AutocompletionResult> emptyOraclesResult
    {
        get
        {
            if (dict.TryGetValue("initialOracles", out Task<AutocompletionResult> result)) return result;

            var oracles = Db.Oracles.Where(o => o.Name == "Pay the Price" || o.OracleInfo.Name == "Core" || o.Name == "Space Sighting").AsEnumerable();
            var list = oracles
                .SelectMany(x => GetOracleAutocompleteResults(x))
                .OrderBy(x => //Todo this is really lazy ordering, but so is the rest of this getter's code.
                    x.Name == "Pay the Price" ? 1 :
                    x.Name.Contains("Space Sighting") ? 3 :
                    2)
                .Take(SelectMenuBuilder.MaxOptionCount);

            result = Task.FromResult(AutocompletionResult.FromSuccess(list));

            dict.Add("initialOracles", result);

            return result;
        }
    }

    public EFContext Db { get; set; }
    public ILogger<OracleAutocomplete> logger { get; set; }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            List<AutocompleteResult> successList = new List<AutocompleteResult>();

            var value = autocompleteInteraction.Data.Current.Value as string;

            if (string.IsNullOrEmpty(value))
            {
                return emptyOraclesResult;
            }

            var oracles = Db.Oracles.Where(x => Regex.IsMatch(x.Name, $@"\b(?i){value}") || Regex.IsMatch(x.OracleInfo.Name, $@"\b(?i){value}")).AsEnumerable();
            successList = oracles.SelectMany(x => GetOracleAutocompleteResults(x)).ToList();

            var subcategories = Db.Subcategory.Where(x => Regex.IsMatch(x.Name, $@"\b(?i){value}"));
            successList.AddRange(subcategories.Select(x => new AutocompleteResult(x.Name, $"subcat:{x.Id}")));

            return Task.FromResult(AutocompletionResult.FromSuccess(successList.Take(SelectMenuBuilder.MaxOptionCount)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }

    private IEnumerable<AutocompleteResult> GetOracleAutocompleteResults(Oracle oracle)
    {
        var list = new List<AutocompleteResult>();
        if (oracle.Tables?.Count > 0)
        {
            foreach (var t in oracle.Tables)
            {
                list.Add(new AutocompleteResult(GetOracleDisplayName(oracle, t), $"tables:{t.Id}"));
            }
        }
        else
        {
            list.Add(new AutocompleteResult(GetOracleDisplayName(oracle), $"oracle:{oracle.Id}"));
        }
        return list;
    }

    private string GetOracleDisplayName(Oracle oracle, Tables t = null)
    {
        string name = oracle.Name;
        if (oracle.Subcategory != null) name = $"{oracle.Subcategory.Name} - {oracle.Name}";

        if (t != null) name += $" - {t.Name}";

        return name;
    }

    protected override string GetLogString(IInteractionContext context) => $"Accessing DB from {context.Guild}-{context.Channel}";
}
