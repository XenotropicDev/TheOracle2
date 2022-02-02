using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.ActionRoller;
using TheOracle2.Commands;
using TheOracle2.UserContent;

namespace TheOracle2;

public class OracleCommand : InteractionModuleBase
{
    public OracleCommand(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        Random = random;
    }

    public EFContext DbContext { get; }
    public Random Random { get; }

    [SlashCommand("oracle", "Roll on an oracle table. To ask a yes/no question, use /ask.")]
    public async Task RollOracle([Autocomplete(typeof(OracleAutocomplete))] string oracle)
    {
        var entityItem = new DiscordOracleEntity(oracle, DbContext, Random);

        await RespondAsync(embeds: entityItem.GetEmbeds(), ephemeral: entityItem.IsEphemeral, components: entityItem.GetComponents());
    }
}

public class OracleComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public TableRollerFactory RollerFactory { get; }

    public OracleComponents(EFContext dbContext, Random random)
    {
        DbContext = dbContext;
        RollerFactory = new TableRollerFactory(dbContext, random);
    }

    public EFContext DbContext { get; }

    [ComponentInteraction("add-oracle-select")]
    public async Task FollowUp(string[] values)
    {
        var builder = Context.Interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
        foreach (var value in values)
        {
            var roll = RollerFactory.GetRoller(value).Build();

            builder = DiscordOracleBuilder.AddFieldsToBuilder(roll, builder);
        }

        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Embeds = new Embed[] { builder.Build() };
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("tables-oracle-*")]
    public async Task SelectedTableRoll(string oracleId, string[] values)
    {
        int.TryParse(values.FirstOrDefault(), out int Id);

        var table = DbContext.Tables.Find(Id);

        var rollResult = RollerFactory.GetRoller(table).Build();

        var ob = new DiscordOracleBuilder(rollResult).Build();

        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Embed = ob.EmbedBuilder.Build();
            msg.Components = ob.ComponentBuilder.Build();
            msg.Content = "";
        }).ConfigureAwait(false);
    }
}
