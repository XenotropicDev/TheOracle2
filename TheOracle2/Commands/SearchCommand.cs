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
    IDiscordEntity entityItem = null;
    switch (searchType)
    {
      case GameEntityType.Oracle:
        entityItem = new DiscordOracleEntity(query, Db, Random);
        break;

      case GameEntityType.Reference:
        if (!int.TryParse(query, out var ReferenceId)) break;
        entityItem = new DiscordMoveEntity(Db.Moves.Find(ReferenceId));
        break;

      case GameEntityType.Asset:
        if (!int.TryParse(query, out var assetId)) break;
        entityItem = new DiscordAssetEntity(Db.Assets.Find(assetId));
        break;

      default:
        break;
    }

    if (entityItem != null)
    {
      await RespondAsync(entityItem.GetDiscordMessage(), embeds: entityItem.GetEmbeds(), ephemeral: entityItem.IsEphemeral, components: entityItem.GetComponents());
      return;
    }

    await RespondAsync($"{query} is not a valid {searchType} id");
  }
}