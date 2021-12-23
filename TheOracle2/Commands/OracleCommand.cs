using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
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

                var requiredOption = new SlashCommandOptionBuilder()
                    .WithName("subcategory")
                    .WithDescription($"Lists subcategory options for the oracle roll")
                    .WithType(ApplicationCommandOptionType.SubCommand);

                foreach (var subCat in oracleInfo.Subcategories)
                {
                    var oracleChoiceOption = new SlashCommandOptionBuilder()
                        .WithName(subCat.Name.Replace(" ", "-").ToLower())
                        .WithDescription($"Oracle to roll")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Integer);

                    foreach (var oracle in subCat.Oracles)
                    {
                        oracleChoiceOption.AddChoice(oracle.Name, oracle.Id);
                    }

                    requiredOption.AddOption(oracleChoiceOption);
                }
                
                topLevelOption.AddOption(requiredOption);
            }

            if (topLevelOption.Type == ApplicationCommandOptionType.SubCommandGroup)
            {
                var subcommand = new SlashCommandOptionBuilder()
                    .WithName("oracle")
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
                if (!int.TryParse(v, out var oracleId)) throw new ArgumentException($"Unknown {nameof(Oracle)} with Id {v}");
                var oracle = DbContext.Oracles.Find(oracleId);

                //Todo: Add oracle field
            }

            await Context.Interaction.UpdateAsync(msg => msg.Embeds = new Embed[] { embed.Build() }).ConfigureAwait(false);
        }
    }
}