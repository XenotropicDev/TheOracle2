using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.UserContent;

namespace TheOracle2;

public class OracleCommand : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>, ISlashCommand
{
    public OracleCommand(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        this.random = random;
        Roller = new OracleRollerService(random, dbContext);
    }

    private SocketSlashCommand SlashCommandContext;
    private readonly Random random;

    public OracleRollerService Roller { get; }
    public EFContext DbContext { get; }

    [OracleSlashCommand("oracle")]
    public async Task RollOracle()
    {
        int Id;
        if (SlashCommandContext.Data.Options.FirstOrDefault().Type == ApplicationCommandOptionType.SubCommandGroup)
        {
            Id = Convert.ToInt32(SlashCommandContext.Data.Options.FirstOrDefault().Options.FirstOrDefault().Options.FirstOrDefault().Value);
        }
        else
        {
            Id = Convert.ToInt32(SlashCommandContext.Data.Options.FirstOrDefault().Options.FirstOrDefault().Value);
        }

        var oracle = DbContext.Oracles.Find(Id);
        var rollResult = Roller.Roll(oracle);

        await SlashCommandContext.RespondAsync(embed: rollResult.GetEmbedBuilder().Build(), components: rollResult.GetComponentBuilder()?.Build()).ConfigureAwait(false);
    }

    public IList<SlashCommandBuilder> GetCommandBuilders()
    {
        var command = new SlashCommandBuilder()
            .WithName("oracle")
            .WithDescription("Rolls an oracle table");

        foreach (var oracleInfo in DbContext.OracleInfo)
        {
            var topLevelOption = new SlashCommandOptionBuilder()
                .WithName(oracleInfo.Name.Replace(" ", "-").ToLower())
                .WithDescription($"{oracleInfo.Name} Oracles")
                .WithType(ApplicationCommandOptionType.SubCommand)
                ;

            //Add the base oracles first
            var oracleOption = new SlashCommandOptionBuilder()
                .WithName("oracle")
                .WithDescription($"Oracle to roll")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.Integer);

            foreach (var oracle in oracleInfo.Oracles)
            {
                oracleOption.AddChoice(oracle.Name, oracle.Id);
            }

            //Add any subcategories and their oracles second
            if (oracleInfo.Subcategories?.Count > 0)
            {
                topLevelOption.WithType(ApplicationCommandOptionType.SubCommandGroup);

                foreach (var subcat in oracleInfo.Subcategories)
                {
                    var subcatOption = new SlashCommandOptionBuilder()
                        .WithName(subcat.Name.Replace(" ", "-").ToLower())
                        .WithDescription(subcat.Name)
                        .WithType(ApplicationCommandOptionType.SubCommand);

                    var oracleChoiceOption = new SlashCommandOptionBuilder()
                        .WithName("oracle")
                        .WithDescription($"Oracle to roll")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer);

                    foreach (var oracle in subcat.Oracles)
                    {
                        oracleChoiceOption.AddChoice(oracle.Name, oracle.Id);
                    }

                    subcatOption.AddOption(oracleChoiceOption);
                    topLevelOption.AddOption(subcatOption);
                }
            }

            if (topLevelOption.Type == ApplicationCommandOptionType.SubCommandGroup)
            {
                var subcommand = new SlashCommandOptionBuilder()
                    .WithName("main")
                    .WithDescription($"Lists the main oracle rolls.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(oracleOption);
                topLevelOption.AddOption(subcommand);
            }
            else
            {
                topLevelOption.AddOption(oracleOption);
            }

            command.AddOption(topLevelOption);
        }

        return new List<SlashCommandBuilder>() { command };
    }

    public void SetCommandContext(SocketSlashCommand slashCommandContext) => this.SlashCommandContext = slashCommandContext;

    [ComponentInteraction("oracle-pair:*")]
    public async Task AddPair(string idStr)
    {
        //Todo: add support for this. Is it even support in dataforged any more?
        if (!int.TryParse(idStr, out int id)) throw new ArgumentException($"Unknown id '{idStr}'");
        var Oracle = DbContext.Oracles.Find(id);

        var embed = Context.Interaction.Message?.Embeds?.FirstOrDefault()?.ToEmbedBuilder();
        if (embed != null)
        {
            
            await Context.Interaction.UpdateAsync(msg => msg.Embeds = new Embed[] { embed.Build() }).ConfigureAwait(false);
        }
    }

    [ComponentInteraction("oracle-followup:*")]
    public async Task FollowUp(string idStr)
    {
        //await DeferAsync();
        if (!int.TryParse(idStr, out int id)) throw new ArgumentException($"Unknown id '{idStr}'");
        var Oracle = DbContext.Oracles.Find(id);

        
    }
}