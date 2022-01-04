using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using TheOracle2.UserContent;

namespace TheOracle2.Commands;

public class DbAutocompleteHandler : AutocompleteHandler
{
    public EFContext Db { get; set; }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
           var value = autocompleteInteraction.Data.Current.Value as string;

            if (string.IsNullOrEmpty(value))
                return Task.FromResult(AutocompletionResult.FromSuccess());

            var matches = Db.Oracles.Where(x => EF.Functions.Like(x.Name, $"{value}%"));
            var list = matches.ToList();
            var successList = matches.Select(x => new AutocompleteResult(x.Name, x.Id.ToString())).Take(SelectMenuBuilder.MaxOptionCount);
            return Task.FromResult(AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }

    protected override string GetLogString(IInteractionContext context) => $"Accessing DB from {context.Guild}-{context.Channel}";
}


public class DatabaseModule : InteractionModuleBase<SocketInteractionContext>
{
    public EFContext Db { get; set; }

    [SlashCommand("search", "Get an item from the database")]
    public async Task GetDbItem([Autocomplete(typeof(DbAutocompleteHandler))] string autoId)
    {
        if (int.TryParse(autoId, out var id))
        {
            //await DeferAsync();

            var item = Db.Oracles.Find(id);

            await RespondAsync($"Id: {item.Id}, Name: {item.Name}");
        }
        else
            await RespondAsync($"{autoId} is not a valid oracle id");
    }
}