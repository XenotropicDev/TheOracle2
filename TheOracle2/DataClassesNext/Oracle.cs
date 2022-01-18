using System.Linq;

namespace TheOracle2.DataClassesNext;

public class OracleNext
{
  public EmbedBuilder ToEmbed()
  {
    EmbedBuilder embed = new EmbedBuilder()
        .WithAuthor(PathTo)
        .WithTitle(DisplayName ?? Name);
    if (Source != null)
    {
      embed.WithFooter(Source.ToString());
    }
    if (Table != null)
    {
      embed.WithDescription(Table.ToString());
    }
    if (Oracles != null)
    {
      foreach (OracleNext oracle in Oracles)
      {
        if (oracle.Table != null)
        {
          embed.AddField(oracle.Name, oracle.Table.ToString());
        }
      }
    }
    return embed;
  }

  public string Name { get; set; }

  [JsonProperty("Display name")]
  public string DisplayName { get; set; }

  public IList<string> Aliases { get; set; }

  public string Description { get; set; }

  public Source Source { get; set; }

  public OracleUsage Usage { get; set; }

  public IList<OracleCategory> Categories { get; set; }

  public IList<OracleNext> Oracles { get; set; }

  public RollableTable Table { get; set; }

  [JsonProperty("_path")]
  public string Path { get; set; }

  [JsonIgnore]
  public string PathTo { get => string.Join(" / ", Path.Split(" / ")[0..^1]); }

}
