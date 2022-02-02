﻿using System.Text.RegularExpressions;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using TheOracle2.UserContent;

namespace TheOracle2.Commands;

public class EntityAutocomplete : AutocompleteHandler
{
    private static readonly Dictionary<string, Task<AutocompletionResult>> dict = new Dictionary<string, Task<AutocompletionResult>>();

    public EFContext Db { get; set; }
    public ILogger<AssetAutocomplete> logger { get; set; }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();

            var value = autocompleteInteraction.Data.Current.Value as string;

            if (string.IsNullOrEmpty(value))
            {
                successList = Db.OracleTemplates.Select(x => new AutocompleteResult(x.EntityName, x.Id.ToString())).Take(SelectMenuBuilder.MaxOptionCount);

                return Task.FromResult(AutocompletionResult.FromSuccess(successList));
            }

            var templates = Db.OracleTemplates.Where(x => Regex.IsMatch(x.EntityName, $@"\b(?i){value}"));
            successList = templates.Select(x => new AutocompleteResult(x.EntityName, x.Id.ToString())).Take(SelectMenuBuilder.MaxOptionCount);

            return Task.FromResult(AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }

    protected override string GetLogString(IInteractionContext context) => $"Accessing DB from {context.Guild}-{context.Channel}";
}
