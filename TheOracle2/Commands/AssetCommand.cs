using Discord.Interactions;
using Discord.WebSocket;
using OracleData;
using TheOracle2.UserContent;

namespace TheOracle2;

public class AssetCommand : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>, ISlashCommand
{
    public AssetCommand(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    private SocketSlashCommand SlashCommandContext;
    public EFContext DbContext { get; }

    [OracleSlashCommand("asset")]
    public async Task BuildAsset()
    {
        int Id = Convert.ToInt32(SlashCommandContext.Data.Options.FirstOrDefault().Options.FirstOrDefault().Value);

        var asset = DbContext.Assets.Find(Id);

        ComponentBuilder compBuilder = null;

        var builder = new EmbedBuilder()
            .WithAuthor($"Asset: {asset.Category}")
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
                    //.WithDefault(ability.Enabled)
                    );

                if (ability.Enabled)
                {
                    description += ability.Text + "\n\n";
                }
            }
            builder.WithDescription(description);
            compBuilder.WithSelectMenu(select);
        }

        if (asset.Counter != null)
        {
            compBuilder ??= new ComponentBuilder();

            compBuilder.WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId("asset-counter-select")
                .WithPlaceholder($"{asset.Counter.Name} {asset.Counter.StartsAt} / {asset.Counter.Max}")
                .WithMinValues(1)
                .WithMaxValues(1)
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"+1 {asset.Counter.Name}").WithValue("asset-counter-up"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"-1 {asset.Counter.Name}").WithValue("asset-counter-down"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"Roll {asset.Counter.Name}").WithValue("asset-counter-roll"))
                );
        }

        if (asset.ConditionMeter != null)
        {
            compBuilder ??= new ComponentBuilder();
            compBuilder.WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId("asset-condition-select")
                .WithPlaceholder($"{asset.ConditionMeter.Name} {asset.ConditionMeter.StartsAt} / {asset.ConditionMeter.Max}")
                .WithMinValues(1)
                .WithMaxValues(1)
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"+1 {asset.ConditionMeter.Name}").WithValue("asset-condition-up"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"-1 {asset.ConditionMeter.Name}").WithValue("asset-condition-down"))
                .AddOption(new SelectMenuOptionBuilder().WithLabel($"Roll {asset.ConditionMeter.Name}").WithValue("asset-condition-roll"))
                );
        }

        await SlashCommandContext.RespondAsync(embed: builder.Build(), components: compBuilder?.Build()).ConfigureAwait(false);
    }

    public IList<SlashCommandBuilder> GetCommandBuilders()
    {
        var command = new SlashCommandBuilder()
            .WithName("asset")
            .WithDescription("Generates an asset");

        foreach (var category in DbContext.Assets.Select(a => a.Category).Distinct())
        {
            var chunkedList = DbContext.Assets.ToList()
                .Where(a => a.Category == category && a.Id != 0)
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

    [ComponentInteraction("asset-condition-select")]
    public async Task ConditionSelection(string[] values)
    {
        await DeferAsync();
        await RespondAsync();
    }

    [ComponentInteraction("asset-counter-select")]
    public async Task CounterSelection(string[] values)
    {
        await DeferAsync();
        await RespondAsync();
    }

    [ComponentInteraction("asset-ability-select:*")]
    public async Task AbilitySelection(string idStr, string[] values)
    {
        //await DeferAsync();
        if (!int.TryParse(idStr, out int id)) throw new ArgumentException($"Unknown id '{idStr}'");
        var asset = DbContext.Assets.Find(id);

        var embed = Context.Interaction.Message?.Embeds?.FirstOrDefault()?.ToEmbedBuilder();
        if (embed != null)
        {
            string desc = string.Empty;
            foreach (var v in values)
            {
                if (!int.TryParse(v, out var abilityId)) throw new ArgumentException($"Unknown {nameof(Ability)} with Id {v}");
                var ability = DbContext.AssetAbilities.Find(abilityId);

                desc += ability.Text + "\n\n";
            }
            embed.WithDescription(desc);

            await Context.Interaction.UpdateAsync(msg => msg.Embeds = new Embed[] { embed.Build() }).ConfigureAwait(false);
        }
    }
}