using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TheOracle2.UserContent;

namespace TheOracle2.Commands;

public class SearchCommandAutocomplete : AutocompleteHandler
{
    public EFContext Db { get; set; }
    public ILogger<SearchCommandAutocomplete> logger { get; set; }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            Enum.TryParse<GameEntityType>(autocompleteInteraction.Data.Options.FirstOrDefault().Value.ToString(), out var entityType);
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();

            var value = autocompleteInteraction.Data.Current.Value as string;

            if (string.IsNullOrEmpty(value))
                return Task.FromResult(AutocompletionResult.FromSuccess());

            var sw = Stopwatch.StartNew();
            switch (entityType)
            {
                case GameEntityType.Oracle:
                    var oracles = Db.Oracles.Where(x => EF.Functions.Like(x.Name, $"{value}%"));
                    successList = oracles.Select(x => new AutocompleteResult(x.Name, x.Id.ToString())).Take(SelectMenuBuilder.MaxOptionCount);
                    break;

                case GameEntityType.Reference:
                    var references = Db.Moves.Where(x => EF.Functions.Like(x.Name, $"{value}%"));
                    successList = references.Select(x => new AutocompleteResult(x.Name, x.Id.ToString())).Take(SelectMenuBuilder.MaxOptionCount);
                    break;

                case GameEntityType.Asset:
                    var assets = Db.Assets.Where(x => EF.Functions.Like(x.Name, $"{value}%"));
                    successList = assets.Select(x => new AutocompleteResult(x.Name, x.Id.ToString())).Take(SelectMenuBuilder.MaxOptionCount);
                    break;

                default:
                    break;
            }

            _ = successList.ToList();
            sw.Stop();
            logger.LogInformation($"{nameof(GenerateSuggestionsAsync)} took {sw.ElapsedMilliseconds}ms for '{value}'");
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
    public Random Random { get; set; }

    [SlashCommand("search", "Get an item from the database")]
    public async Task GetDbItem(GameEntityType searchType, [Autocomplete(typeof(SearchCommandAutocomplete))] string query)
    {
        if (int.TryParse(query, out var id))
        {
            IDiscordEntity entityItem;
            switch (searchType)
            {
                case GameEntityType.Oracle:
                    entityItem = new DiscordOracleEntity(Db.Oracles.Find(id), Db, Random);
                    break;

                case GameEntityType.Reference:
                    entityItem = new DiscordMoveEntity(Db.Moves.Find(id));
                    break;

                case GameEntityType.Asset:
                    entityItem = new DiscordAssetEntity(Db.Assets.Find(id));
                    break;

                default:
                    return;
            }

            if (entityItem != null) await RespondAsync(entityItem.GetDiscordMessage(), embeds: entityItem.GetEmbeds(), ephemeral: entityItem.IsEphemeral, components: entityItem.GetComponents());
        }

        await RespondAsync($"{query} is not a valid {searchType} id");
    }
}