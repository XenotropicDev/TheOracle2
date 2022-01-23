using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.Commands;
using TheOracle2.DiscordHelpers;
using TheOracle2.UserContent;

namespace TheOracle2;

public class AssetCommand : InteractionModuleBase
{
    public AssetCommand(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        Random = random;
    }

    public EFContext DbContext { get; }
    public Random Random { get; }

    [SlashCommand("asset", "Display an interactive asset card (p. 50)")]
    public async Task PostAsset([Autocomplete(typeof(AssetAutocomplete))] string asset)
    {
        var assetData = DbContext.Assets.Find(asset);
        var entityItem = new DiscordAssetEntity(assetData);

        await RespondAsync(embeds: entityItem.GetEmbeds(), ephemeral: entityItem.IsEphemeral, components: entityItem.GetComponents());
    }
}

public class AssetComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public AssetComponents(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        Random = random;
    }

    public EFContext DbContext { get; }
    public Random Random { get; }

    [ComponentInteraction("asset-condition-select:*")]
    public async Task ConditionSelection(string assetId, string[] values)
    {
        // if (!int.TryParse(assetId, out var id)) throw new ArgumentException($"Unknown asset id {assetId}");
        var asset = DbContext.Assets.Find(assetId);

        var embed = Context.Interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
        ComponentBuilder component = ComponentBuilder.FromMessage(Context.Interaction.Message);
        var conditionField = embed.Fields.Find(f => f.Name == asset.ConditionMeter.Name);

        //clear all the selected items
        foreach (var row in component.ActionRows)
        {
            var select = row.Components.OfType<SelectMenuComponent>().FirstOrDefault(c => c.CustomId == Context.Interaction.Data.CustomId);
            if (select == null) continue;

            var selectBuilder = select.ToBuilder();
            selectBuilder.Options.ForEach(o => o.WithDefault(false));
            row.WithComponents(new List<IMessageComponent> { selectBuilder.Build() });
        }
        if (asset.ConditionMeter.Conditions.Contains(conditionField.Value.ToString()))
        {
            conditionField.Value = 0;
        }

        switch (values.FirstOrDefault())
        {
            case "asset-condition-up":
                embed.ChangeNumericField(asset.ConditionMeter.Name, 1, 0, asset.ConditionMeter.Max);
                break;

            case "asset-condition-down":
                embed.ChangeNumericField(asset.ConditionMeter.Name, -1, 0, asset.ConditionMeter.Max);
                break;

            case "asset-condition-roll":
                if (!int.TryParse(conditionField.Value.ToString(), out int value)) throw new ArgumentException();
                var roller = new ActionRoll(Random, value, 0, description: $"{asset.ConditionMeter.Name} Roll");
                await RespondAsync(embed: roller.ToEmbed().Build()).ConfigureAwait(false);
                await Context.Interaction.Message.ModifyAsync(msg => msg.Components = component.Build());
                return;

            case string s when (asset.ConditionMeter?.Conditions?.Contains(s) ?? false):
                component.MarkSelectionByOptionId(s);
                conditionField.Value = s;
                break;

            default:
                break;
        }

        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Embeds = new Embed[] { embed.Build() };
            msg.Components = component.Build();
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("asset-counter-select:*")]
    public async Task CounterSelection(string assetId, string[] values)
    {
        // if (!int.TryParse(assetId, out var id)) throw new ArgumentException($"Unknown asset id {assetId}");
        var asset = DbContext.Assets.Find(assetId);

        var embed = Context.Interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
        ComponentBuilder component = ComponentBuilder.FromMessage(Context.Interaction.Message);

        switch (values.FirstOrDefault())
        {
            case "asset-counter-up":
                break;

            case "asset-counter-down":
                break;

            case "asset-counter-roll":
                break;

            default:
                break;
        }

        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Embeds = new Embed[] { embed.Build() };
            msg.Components = component.Build();
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("asset-ability-select:*")]
    public async Task AbilitySelection(string idStr, string[] selections)
    {
        //await DeferAsync();
        // if (!int.TryParse(idStr, out int id)) throw new ArgumentException($"Unknown id '{idStr}'");
        var asset = DbContext.Assets.Find(idStr);

        var embed = Context.Interaction.Message?.Embeds?.FirstOrDefault()?.ToEmbedBuilder();
        var component = ComponentBuilder.FromMessage(Context.Interaction.Message);

        if (embed != null)
        {
            string desc = string.Empty;
            foreach (var v in selections)
            {
                // if (!int.TryParse(v, out var abilityId)) throw new ArgumentException($"Unknown {nameof(Ability)} with Id {v}");
                // var ability = DbContext.AssetAbilities.Find(abilityId);
                var ability = DbContext.AssetAbilities.Find(v);

                desc += $"⬢ {ability.Text}\n\n";

                component.MarkSelectionByOptionId(v);
            }
            embed.WithDescription(desc);

            await Context.Interaction.UpdateAsync(msg =>
            {
                msg.Embeds = new Embed[] { embed.Build() };
                msg.Components = component.Build();
            }).ConfigureAwait(false);
        }
    }
}
