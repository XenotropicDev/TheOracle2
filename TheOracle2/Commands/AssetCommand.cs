using Discord.Interactions;
using Discord.WebSocket;
using OracleData;
using TheOracle2.UserContent;

namespace TheOracle2;

public class AssetCommand : ISlashCommand
{
    public AssetCommand(UserContent.EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public SocketSlashCommand SlashCommandContext { get; set; }
    public EFContext DbContext { get; }

    [OracleSlashCommand("asset")]
    public async Task BuildAsset()
    {
        //await Context.DeferAsync().ConfigureAwait(false);
        int Id = Convert.ToInt32(SlashCommandContext.Data.Options.FirstOrDefault().Options.FirstOrDefault().Value);

        var asset = DbContext.Assets.Find(Id);

        var compBuilder = new ComponentBuilder()
            .WithButton("Asset Button", "custom-id");

        EmbedBuilder builder = new EmbedBuilder();
        builder.WithAuthor($"Asset: {asset.Category}");
        builder.WithTitle(asset.Name);

        int abilityNumber = 0;
        foreach (var abl in asset.Abilities ?? new List<Ability>())
        {
            abilityNumber++;
            string abilityText = abl.Text;
            string label = $"{abilityNumber}. {(abl.Enabled ? "X" : "O")}";

            builder.AddField(label, abilityText);
        }

        await SlashCommandContext.RespondAsync(embed: builder.Build(), component: compBuilder.Build());
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
}
