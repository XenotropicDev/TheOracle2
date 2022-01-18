using System.ComponentModel;

namespace TheOracle2.DataClassesNext;
public class MultipleRolls
{

  [DefaultValue(2)]
  public int Amount { get; set; }

  [JsonProperty("Allow duplicates")]
  [DefaultValue(false)]
  public bool AllowDuplicates { get; set; }
}
