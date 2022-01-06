using Discord.Interactions;
using TheOracle2.UserContent;

namespace TheOracle2.Commands;

public class SearchCommand : InteractionModuleBase<SocketInteractionContext>
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

            if (entityItem != null)
            {
                await RespondAsync(entityItem.GetDiscordMessage(), embeds: entityItem.GetEmbeds(), ephemeral: entityItem.IsEphemeral, components: entityItem.GetComponents());
                return;
            }
        }

        await RespondAsync($"{query} is not a valid {searchType} id");
    }
}