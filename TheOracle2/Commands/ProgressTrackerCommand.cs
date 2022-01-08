using Discord.Interactions;
using Discord.WebSocket;

namespace TheOracle2.Commands;

public class ProgressTrackerCommand : InteractionModuleBase
{
  public const int dangerousTicks = 8;
  public const int epicTicks = 1;
  public const int extremeTicks = 2;
  public const int formidableTicks = 4;
  public const int totalTicks = 40;
  public const int troublesomeTicks = 12;
  private readonly Random random;

  public ProgressTrackerCommand(Random random)
  {
    this.random = random;
  }

  [SlashCommand("track", "Creates a generic tracker for things like vows, expeditions, and combat")]
  public async Task PostTracker(string Description, ChallengeRank Rank)
  {
    var embed = new EmbedBuilder()
        .WithTitle("Progress Tracker")
        .WithDescription(Description)
        .WithFields(new EmbedFieldBuilder()
        {
          Name = "Difficulty",
          Value = Rank.ToString(),
          IsInline = true
        })
        .WithFields(new EmbedFieldBuilder()
        {
          Name = "Progress Bar",
          Value = GetProgressGraphic(0),
          IsInline = true
        })
        .WithFields(new EmbedFieldBuilder()
        {
          Name = "Progress Amount",
          Value = BuildProgressAmount(0),
          IsInline = true
        })
        .WithFooter($"Ticks: 0")
        .Build();

    var compBuilder = new ComponentBuilder()
        .WithButton("-", "lose-progress", row: 0, style: ButtonStyle.Danger)
        .WithButton("+", "add-progress", row: 0, style: ButtonStyle.Success)
        .WithButton("#", "add-full-progress", row: 0, style: ButtonStyle.Secondary)
        .WithButton(customId: "roll-progress", row: 0, style: ButtonStyle.Secondary, emote: new Emoji("🎲"))
        ;

    await RespondAsync(embed: embed, components: compBuilder.Build()).ConfigureAwait(false);
  }

  public virtual string GetProgressGraphic(int Ticks)
  {
    //Use standard characters as stand-ins so that we can do easy string math
    string fill = new string('#', (int)Math.Floor(Ticks / 4d));
    string finalTickMark = ((Ticks % 4) == 1) ? "-" : ((Ticks % 4) == 2) ? "+" : ((Ticks % 4) == 3) ? "*" : string.Empty;
    fill = (fill + finalTickMark).PadRight(10, '·');
    fill += "\u200C"; //special hidden character for mobile formatting small emojis

    fill = String.Join(' ', fill.ToCharArray()); //Add spaces between each character

    //Replace all the stand-in characters with emojis
    fill = fill.Replace("·", "<:progress0:880599822468534374>");
    fill = fill.Replace("-", "<:progress1:880599822736965702>");
    fill = fill.Replace("+", "<:progress2:880599822724390922>");
    fill = fill.Replace("*", "<:progress3:880599822736957470>");
    fill = fill.Replace("#", "<:progress4:880599822820864060>");

    return fill;
  }

  public virtual string BuildProgressAmount(int ticks)
  {
    return $"{(int)(ticks / 4)}/10";
  }

  [ComponentInteraction("add-progress")]
  public async Task AddProgress()
  {
    var interaction = Context.Interaction as SocketMessageComponent;

    await interaction.UpdateAsync(msg =>
    {
      var embed = interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
      msg.Embeds = ChangeProgress(embed);
    }).ConfigureAwait(false);
  }

  [ComponentInteraction("lose-progress")]
  public async Task LoseProgress()
  {
    var interaction = Context.Interaction as SocketMessageComponent;

    await interaction.UpdateAsync(msg =>
    {
      var embed = interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
      msg.Embeds = ChangeProgress(embed, -1);
    }).ConfigureAwait(false);
  }

  [ComponentInteraction("add-full-progress")]
  public async Task AddFullProgress()
  {
    var interaction = Context.Interaction as SocketMessageComponent;

    await interaction.UpdateAsync(msg =>
    {
      var embed = interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
      msg.Embeds = ChangeProgress(embed, exactAmount: 4);
    }).ConfigureAwait(false);
  }

  [ComponentInteraction("roll-progress")]
  public async Task RollProgress()
  {
    var interaction = Context.Interaction as SocketMessageComponent;
    if (!int.TryParse(interaction.Message.Embeds.FirstOrDefault().Footer?.Text?.Replace("Ticks: ", ""), out int ticks))
    {
      await RespondAsync("Unknown progress type");
    }
    var roll = new ProgressRoll(random, ticks / 4, interaction.Message.Embeds.FirstOrDefault().Description);

    await interaction.RespondAsync(embed: roll.ToEmbed().WithAuthor($"Progress Roll").Build()).ConfigureAwait(false);
  }

  private Embed[] ChangeProgress(EmbedBuilder embed, int delta = 1, int? exactAmount = null)
  {
    if (!int.TryParse(embed.Footer.Text.Replace("Ticks: ", ""), out int ticks)) return new Embed[] { embed.Build() };
    if (!Enum.TryParse<ChallengeRank>(embed.Fields.Find(f => f.Name == "Difficulty")?.Value.ToString(), out var rank)) return new Embed[] { embed.Build() };

    ticks += (exactAmount == null) ? TicksToAdd(rank) * delta : exactAmount.Value;
    embed.WithFooter($"Ticks: {ticks}");

    int amountIndex = embed.Fields.FindIndex(f => f.Name == "Progress Amount");
    embed.Fields[amountIndex].Value = BuildProgressAmount(ticks);

    int barIndex = embed.Fields.FindIndex(f => f.Name == "Progress Bar");
    embed.Fields[barIndex].Value = GetProgressGraphic(ticks);

    return new Embed[] { embed.Build() };
  }

  public virtual int TicksToAdd(ChallengeRank rank)
  {
    switch (rank)
    {
      case ChallengeRank.Troublesome:
        return troublesomeTicks;

      case ChallengeRank.Dangerous:
        return dangerousTicks;

      case ChallengeRank.Formidable:
        return formidableTicks;

      case ChallengeRank.Extreme:
        return extremeTicks;

      case ChallengeRank.Epic:
        return epicTicks;

      default:
        return 0;
    }
  }
}