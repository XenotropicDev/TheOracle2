﻿using Discord.Interactions;
using Discord.WebSocket;
using OracleData;
using TheOracle2.UserContent;

namespace TheOracle2;

public class AssetCommand : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>, ISlashCommand
{
    public AssetCommand(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        this.random = random;
    }

    private SocketSlashCommand SlashCommandContext;
    private readonly Random random;

    public EFContext DbContext { get; }

    [OracleSlashCommand("asset")]
    public async Task BuildAsset()
    {
        int Id = Convert.ToInt32(SlashCommandContext.Data.Options.FirstOrDefault().Options.FirstOrDefault().Value);

        var asset = DbContext.Assets.Find(Id);

        ComponentBuilder compBuilder = null;

        var builder = new EmbedBuilder()
            .WithAuthor($"Asset: {asset.AssetType}")
            .WithTitle(asset.Name);

        if (asset.Abilities != null)
        {
            compBuilder ??= new ComponentBuilder();

            var select = new SelectMenuBuilder()
                .WithCustomId($"asset-ability-select:{asset.Id}")
                .WithPlaceholder($"Ability Selection")
                .WithMinValues(0)
                .WithMaxValues(asset.Abilities.Count);

            string description = string.Empty;
            foreach (var ability in asset.Abilities)
            {
                select.AddOption(new SelectMenuOptionBuilder()
                    .WithLabel($"Ability {asset.Abilities.IndexOf(ability) + 1}")
                    .WithValue($"{ability.Id}")
                    .WithDefault(ability.Enabled)
                    );

                if (ability.Enabled)
                {
                    description += $"⬢ {ability.Text}\n\n";
                }
            }
            builder.WithDescription(description);
            compBuilder.WithSelectMenu(select);
        }

        if (asset.Counter != null)
        {
            compBuilder ??= new ComponentBuilder();

            compBuilder.WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId($"asset-counter-select:{asset.Id}")
                .WithPlaceholder($"{asset.Counter.Name} actions")
                .WithMinValues(1)
                .WithMaxValues(1)
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"+1 {asset.Counter.Name}").WithValue("asset-counter-up"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"-1 {asset.Counter.Name}").WithValue("asset-counter-down"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"Roll {asset.Counter.Name}").WithValue("asset-counter-roll"))
                );
        }

        if (asset.ConditionMeter != null)
        {
            builder.AddField(asset.ConditionMeter.Name, asset.ConditionMeter.StartsAt ?? asset.ConditionMeter.Max, false);

            //todo: show condition in select?
            var select = new SelectMenuBuilder()
                .WithCustomId($"asset-condition-select:{asset.Id}")
                .WithPlaceholder($"{asset.ConditionMeter.Name} actions")
                .WithMinValues(1)
                .WithMaxValues(1)
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"+1 {asset.ConditionMeter.Name}").WithValue("asset-condition-up"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"-1 {asset.ConditionMeter.Name}").WithValue("asset-condition-down"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"Roll {asset.ConditionMeter.Name}").WithValue("asset-condition-roll"));

            if (asset.ConditionMeter.Conditions?.Count > 0)
            {
                foreach (var condition in asset.ConditionMeter.Conditions)
                {
                    select.AddOption(new SelectMenuOptionBuilder().WithLabel($"Condition: {condition}").WithValue(condition));
                }
            }

            compBuilder ??= new ComponentBuilder();
            compBuilder.WithSelectMenu(select);
        }

        await SlashCommandContext.RespondAsync(embed: builder.Build(), components: compBuilder?.Build()).ConfigureAwait(false);
    }

    public IList<SlashCommandBuilder> GetCommandBuilders()
    {
        var command = new SlashCommandBuilder()
            .WithName("asset")
            .WithDescription("Generates an asset");

        foreach (var category in DbContext.Assets.Select(a => a.AssetType).Distinct())
        {
            var chunkedList = DbContext.Assets.ToList()
                .Where(a => a.AssetType == category && a.Id != 0)
                .OrderBy(a => a.Name)
                .Chunk(SlashCommandOptionBuilder.MaxChoiceCount);

            foreach (var assetGroup in chunkedList)
            {
                string name = category.Replace(" ", "-");
                if (chunkedList.Count() > 1)
                {
                    name += $"-{assetGroup.First().Name.Substring(0, 1)}-{assetGroup.Last().Name.Substring(0, 1)}";
                }

                var option = new SlashCommandOptionBuilder()
                    .WithName(name.ToLower())
                    .WithDescription($"{category} assets")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    ;

                var subChoicesOption = new SlashCommandOptionBuilder()
                    .WithName("asset-name")
                    .WithDescription("The name of the asset to be generated")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Integer);

                foreach (var asset in assetGroup)
                {
                    subChoicesOption.AddChoice(asset.Name, asset.Id);
                }
                option.AddOption(subChoicesOption);

                command.AddOption(option);
            }
        }

        return new List<SlashCommandBuilder>() { command };
    }

    public void SetCommandContext(SocketSlashCommand slashCommandContext) => this.SlashCommandContext = slashCommandContext;

    [ComponentInteraction("asset-condition-select:*")]
    public async Task ConditionSelection(string assetId, string[] values)
    {
        if (!int.TryParse(assetId, out var id)) throw new ArgumentException($"Unknown asset id {assetId}");
        var asset = DbContext.Assets.Find(id);

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
                var roller = new ActionRoll(value, 0, text: $"{asset.ConditionMeter.Name} Roll");
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
        if (!int.TryParse(assetId, out var id)) throw new ArgumentException($"Unknown asset id {assetId}");
        var asset = DbContext.Assets.Find(id);

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
        if (!int.TryParse(idStr, out int id)) throw new ArgumentException($"Unknown id '{idStr}'");
        var asset = DbContext.Assets.Find(id);

        var embed = Context.Interaction.Message?.Embeds?.FirstOrDefault()?.ToEmbedBuilder();
        var component = ComponentBuilder.FromMessage(Context.Interaction.Message);

        if (embed != null)
        {
            string desc = string.Empty;
            foreach (var v in selections)
            {
                if (!int.TryParse(v, out var abilityId)) throw new ArgumentException($"Unknown {nameof(Ability)} with Id {v}");
                var ability = DbContext.AssetAbilities.Find(abilityId);

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