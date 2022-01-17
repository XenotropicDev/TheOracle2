using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.ActionRoller;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2;

public class OracleCommand : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>, ISlashCommand
{
  public OracleCommand(EFContext dbContext, Random random)
  {
    DbContext = dbContext;
    RollerFactory = new TableRollerFactory(dbContext, random);
  }

  private SocketSlashCommand SlashCommandContext;
  public EFContext DbContext { get; }
  public TableRollerFactory RollerFactory { get; }

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
    //IRollerService rollerType = RollerFactory.GetRoller(oracle);

    if (oracle.Tables?.Count > 0)
    {
      var selectMenu = new SelectMenuBuilder()
          .WithPlaceholder($"Select the {oracle.SelectTableBy} type.")
          .WithCustomId($"tables-oracle-{Id}");
      foreach (var table in oracle.Tables)
      {
        selectMenu.AddOption(table.Name, $"{table.Id}");
      }

      await SlashCommandContext.RespondAsync("Please select one", components: new ComponentBuilder().WithSelectMenu(selectMenu).Build());
      return;
    }

    var rollResult = RollerFactory.GetRoller(oracle).Build();

    var ob = new DiscordOracleBuilder(rollResult).Build();

    await SlashCommandContext.RespondAsync(embed: ob.EmbedBuilder.Build(), components: ob.ComponentBuilder.Build()).ConfigureAwait(false);
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
          .WithType(ApplicationCommandOptionType.SubCommand);

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
      AddSubcategories(topLevelOption, oracleInfo);

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

  private SlashCommandOptionBuilder AddSubcategories(SlashCommandOptionBuilder builder, OracleInfo oracleInfo)
  {
    if (oracleInfo.Subcategories == null || oracleInfo.Subcategories?.Count == 0) return builder;

    builder.WithType(ApplicationCommandOptionType.SubCommandGroup);

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
      builder.AddOption(subcatOption);
    }

    return builder;
  }

  //This isn't used because it makes the command too large for discord to use
  private SlashCommandOptionBuilder GetRequiredList(IEnumerable<Oracle> oralces)
  {
    var require = new Dictionary<string, int>();

    string desc = "options";
    foreach (var oracle in oralces.Where(o => o.Tables != null))
    {
      desc = oracle.SelectTableBy ?? desc;

      foreach (var table in oracle.Tables)
      {
        if (table == null || require.ContainsKey(table.Name)) continue;

        require.Add(table.Name, table.Id);
      }
    }

    if (require.Count == 0) return null;

    var builder = new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.Integer).WithName("req").WithDescription(desc);

    foreach (var item in require)
    {
      builder.AddChoice(item.Key, item.Value);
    }

    return builder;
  }

  public void SetCommandContext(SocketSlashCommand slashCommandContext) => this.SlashCommandContext = slashCommandContext;

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

//internal class OracleFetchFactory
//{
//    internal static IOracleFetcher Get(string value)
//    {
//        if (value.StartsWith("oracle:")) return new OracleFetcher();
//        if (value.StartsWith("tables:")) return new TablesFetcher();
//        return new OracleFetcher();
//    }

//    internal static Oracle Fetch(EFContext context, string value)
//    {
//        var fetcher = Get(value);
//        return fetcher.Fetch(context, value);
//    }
//}

//public interface IOracleFetcher
//{
//    public Oracle Fetch(EFContext context, string value);
//}

//public class OracleFetcher : IOracleFetcher
//{
//    public Oracle Fetch(EFContext context, string value)
//    {
//        if (!int.TryParse(value.Replace("oracle:", "", StringComparison.OrdinalIgnoreCase), out int id)) throw new ArgumentException($"Unknown value {value}");
//        return context.Oracles.Find(id);
//    }
//}

//public class TablesFetcher : IOracleFetcher
//{
//    public Oracle Fetch(EFContext context, string value)
//    {
//        if (!int.TryParse(value.Replace("tables:", "", StringComparison.OrdinalIgnoreCase), out int id)) throw new ArgumentException($"Unknown value {value}");
//        return context.Tables.Find(id).Oracle;
//    }
//}