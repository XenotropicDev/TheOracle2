using System.ComponentModel;

namespace TheOracle2.DataClassesNext;
public class AddTemplate
{
  [JsonProperty("Object type")]
  public string ObjectType { get; set; }
}