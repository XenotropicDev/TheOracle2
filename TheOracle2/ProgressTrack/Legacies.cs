namespace TheOracle2.GameObjects;

public class Legacies : List<LegacyTrack>, IWidget
{
  public Legacies(Embed embed)
  {
    foreach (Legacy legacy in Enum.GetValues(typeof(Legacy)))
    {
      var embedField = embed.Fields.FirstOrDefault(field => field.Name.StartsWith(legacy.ToString()));
      Add(new LegacyTrack(embedField));
    }
  }
  public Legacies(params int[] legacyTicks)
  {
    int index = 0;
    foreach (Legacy legacy in Enum.GetValues(typeof(Legacy)))
    {
      Add(new LegacyTrack(legacy, legacyTicks[index]));
      index++;
    }
  }
  public int Xp => this.Select(item => item.Xp).Sum();
  public EmbedBuilder ToEmbed()
  {
    EmbedBuilder embed = IWidget.EmbedStub(this)
      .AddField("XP Earned", Xp.ToString(), true)
      .AddField("XP Spent", "0", true);
    foreach (LegacyTrack legacy in this)
    {
      embed.AddField(legacy.ToEmbedField());
    }
    return embed;
  }
  public string Title => "Legacies";
  public string EmbedCategory { get; set; }
  public string Description { get; set; }
  public string Footer { get; set; }
  public ComponentBuilder MakeComponents()
  {
    SelectMenuBuilder selectMenu = new SelectMenuBuilder()
      .WithPlaceholder("Mark Legacy...")
      .WithCustomId("legacies-menu")
      .WithMinValues(0)
      .WithMaxValues(1);
    foreach (LegacyTrack legacy in this)
    {
      foreach (ChallengeRank rank in Enum.GetValues<ChallengeRank>())
        selectMenu.AddOption(legacy.MarkRewardOption(rank));
    }
    foreach (LegacyTrack legacy in this)
    {
      selectMenu.AddOption(legacy.ClearOption(1));
      selectMenu.AddOption(legacy.ClearOption(ITrack.BoxSize));
    }

    return new ComponentBuilder().WithSelectMenu(selectMenu);
  }
  public void Mark(Legacy legacy, int ticks)
  {
    var targetLegacy = this.FirstOrDefault(item => item.Legacy == legacy);
    targetLegacy.MarkReward(ticks);
  }
}