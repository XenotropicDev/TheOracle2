using Discord.Interactions;
using Discord.WebSocket;
using OracleData;
using TheOracle2.UserContent;

namespace TheOracle2;

public class OracleCommand : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>, ISlashCommand
{
    public OracleCommand(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        this.random = random;
    }

    private SocketSlashCommand SlashCommandContext;
    private readonly Random random;

    public EFContext DbContext { get; }

    [OracleSlashCommand("oracle")]
    public async Task RollOracle()
    {
        int Id = Convert.ToInt32(SlashCommandContext.Data.Options.FirstOrDefault().Options.FirstOrDefault().Value);

        var oracle = DbContext.Oracles.Find(Id);

        ComponentBuilder compBuilder = null;

        var builder = new EmbedBuilder()
            .WithAuthor($"Oracle: {oracle.Category}")
            .WithTitle(oracle.Name);

        var oracleRollResult = oracle.Roll(random);

        await SlashCommandContext.RespondAsync(embed: builder.Build(), components: compBuilder?.Build()).ConfigureAwait(false);
    }

    public IList<SlashCommandBuilder> GetCommandBuilders()
    {
        var command = new SlashCommandBuilder()
            .WithName("oracle")
            .WithDescription("Rolls an oracle table");

        foreach (var category in DbContext.Oracles.Select(a => a.Category).Distinct())
        {
            var chunkedList = DbContext.Oracles.ToList()
                .Where(a => a.Category == category && a.Id != 0)
                .OrderBy(a => a.Name)
                .Chunk(SlashCommandOptionBuilder.MaxChoiceCount);

            foreach (var OracleGroup in chunkedList)
            {
                string name = category.Replace(" ", "-");
                if (chunkedList.Count() > 1)
                {
                    name += $"-{OracleGroup.First().Name.Substring(0, 1)}-{OracleGroup.Last().Name.Substring(0, 1)}";
                }

                var option = new SlashCommandOptionBuilder()
                    .WithName(name.ToLower())
                    .WithDescription($"{category} Oracles")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    ;

                var subChoicesOption = new SlashCommandOptionBuilder()
                    .WithName("oracle-name")
                    .WithDescription("The name of the Oracle to be generated")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Integer);

                foreach (var Oracle in OracleGroup)
                {
                    subChoicesOption.AddChoice(Oracle.Name, Oracle.Id);
                }
                option.AddOption(subChoicesOption);

                command.AddOption(option);
            }
        }

        return new List<SlashCommandBuilder>() { command };
    }

    public void SetCommandContext(SocketSlashCommand slashCommandContext) => this.SlashCommandContext = slashCommandContext;

    [ComponentInteraction("oracle-pair:*")]
    public async Task AddPair(string idStr, string[] values)
    {
        //await DeferAsync();
        if (!int.TryParse(idStr, out int id)) throw new ArgumentException($"Unknown id '{idStr}'");
        var Oracle = DbContext.Oracles.Find(id);

        var embed = Context.Interaction.Message?.Embeds?.FirstOrDefault()?.ToEmbedBuilder();
        if (embed != null)
        {
            string desc = string.Empty;
            foreach (var v in values)
            {
                if (!int.TryParse(v, out var oracleId)) throw new ArgumentException($"Unknown {nameof(Ability)} with Id {v}");
                var oracle = DbContext.Oracles.Find(oracleId);

                //Todo: Add oracle field
            }

            await Context.Interaction.UpdateAsync(msg => msg.Embeds = new Embed[] { embed.Build() }).ConfigureAwait(false);
        }
    }
}