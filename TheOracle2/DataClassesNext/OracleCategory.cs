namespace TheOracle2.DataClassesNext;
public class OracleCategory
{
  public string Name { get; set; }

  [JsonProperty("Display name")]
  public string DisplayName { get; set; }
  public IList<string> Aliases { get; set; }
  public Source Source { get; set; }
  public string Description { get; set; }
  public IList<OracleNext> Oracles { get; set; }
  public IList<OracleCategory> Categories { get; set; }

  [JsonProperty("_path")]
  public string Path { get; set; }
}