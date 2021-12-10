using Discord.WebSocket;

namespace TheOracle2;

public class AskTheOracleCommand : ISlashCommand
{
    public SocketSlashCommand SlashCommandContext { get; set; }

    [OracleSlashCommand("ask")]
    public async Task Ask(UserContent.EFContext test)
    {
        int chance = Convert.ToInt32(SlashCommandContext.Data.Options.FirstOrDefault().Options.FirstOrDefault().Value);

        if (chance > 0)
        {
            Random rnd = new Random();
            var roll = rnd.Next(101);
            string result = (roll >= 100 - chance) ? "Yes" : "No";
            await SlashCommandContext.RespondAsync($"You rolled {roll} VS. {chance}% chance\n**{result}**.").ConfigureAwait(false);
        }
    }

    public IList<SlashCommandBuilder> GetCommandBuilders()
    {
        var command = new SlashCommandBuilder()
            .WithName("ask")
            .WithDescription("Ask the oracle")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("with-likelihood")
                .WithDescription("Ask the oracle based on the predefined likelihoods")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("keyword")
                    .WithDescription("Ask the oracle based on the predefined likelihoods")
                    .WithRequired(true)
                    .AddChoice("Almost Certain", 10)
                    .AddChoice("Likely", 25)
                    .AddChoice("50/50", 50)
                    .AddChoice("Unlikely", 75)
                    .AddChoice("Small Chance", 90)
                    .WithType(ApplicationCommandOptionType.Integer)
                )
            )
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("with-chance")
                .WithDescription("Ask the oracle based on a percentage")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("by-percent")
                    .WithDescription("Ask the oracle based on a given percent change of something happening")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Integer)
                )
            );

        return new List<SlashCommandBuilder>() { command };
    }
}
