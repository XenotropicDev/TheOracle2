using System.ComponentModel;

namespace TheOracle2.DataClassesNext;
public class OracleUsage
{
  public IList<Requires> Requires { get; set; }
  public Suggestions Suggestions { get; set; }

  [DefaultValue(false)]
  public bool Repeatable { get; set; }

  [DefaultValue(false)]
  public bool Initial { get; set; }

  [JsonProperty("Max rolls")]
  [DefaultValue(1)]
  public int MaxRolls { get; set; }


  [JsonProperty("Min rolls")]
  [DefaultValue(1)]
  public int MinRolls { get; set; }


  [JsonProperty("Allow duplicate rolls")]
  [DefaultValue(false)]
  public bool AllowDuplicateRolls { get; set; }
};
