using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace OracleData;

public record Ability
{
  [JsonIgnore]
  public string Id { get; set; }

  [JsonProperty("Alter Moves")]
  public virtual IList<AlterMove> AlterMoves { get; set; }

  [JsonProperty("Alter Properties")]
  public virtual AlterProperties AlterProperties { get; set; }

  [JsonProperty("Counter")]
  public virtual Counter Counter { get; set; }

  [JsonProperty("Enabled")]
  public bool Enabled { get; set; }

  [JsonProperty("Input")]
  public IList<string> Input { get; set; }

  public virtual AssetMove Move { get; set; }

  [JsonProperty("Text")]
  public string Text { get; set; }
}

public record AlterProperties
{
  [JsonIgnore]
  public int Id { get; set; }
  [JsonProperty("Condition Meter")]
  public virtual ConditionMeter ConditionMeter { get; set; }

  [JsonProperty("Track")]
  public virtual Track Track { get; set; }
}

public record AssetRoot
{
  [JsonIgnore]
  public int Id { get; set; }
  [JsonProperty("Assets")]
  public virtual IList<Asset> Assets { get; set; }

  [JsonProperty("Name")]
  public string Name { get; set; }

  [JsonProperty("Source")]
  public virtual Source Source { get; set; }

  [JsonProperty("Tags")]
  public IList<string> Tags { get; set; }
}

public record Asset
{
  [JsonIgnore]
  public string Id { get; set; }

  [JsonIgnore]
  public virtual IList<OracleGuild> OracleGuilds { get; set; }

  [JsonProperty("Abilities")]
  public virtual IList<Ability> Abilities { get; set; }

  [JsonProperty("Aliases")]
  public IList<string> Aliases { get; set; }

  [JsonProperty("Asset Type")]
  public string AssetType { get; set; }

  [JsonProperty("Condition Meter")]
  public virtual ConditionMeter ConditionMeter { get; set; }

  [JsonProperty("Counter")]
  public virtual Counter Counter { get; set; }

  [JsonProperty("Description")]
  public string Description { get; set; }

  [JsonProperty("Input")]
  public IList<string> Input { get; set; }

  public bool Modules { get; set; }

  [JsonProperty("Name")]
  public string Name { get; set; }

  [JsonProperty("Select")]
  public virtual Select Select { get; set; }

  [JsonProperty("Track")]
  public virtual Track Track { get; set; }
}

public class AssetStatOptions
{
  [JsonIgnore]
  public int Id { get; set; }

  public string Select { get; set; }
  public IList<string> Stats { get; set; }
  public IList<string> Resources { get; set; }
  public virtual IList<Special> Special { get; set; }
  public IList<string> Legacies { get; set; }
  public string Selection { get; set; }
}

public class AssetTrigger
{
  [JsonIgnore]
  public int Id { get; set; }

  public string Details { get; set; }
  public string Text { get; set; }

  [JsonProperty("Stat Options")]
  public virtual AssetStatOptions StatOptions { get; set; }
}

public class AssetMove
{
  [JsonIgnore]
  public int Id { get; set; }

  public string Name { get; set; }
  public string Category { get; set; }
  public string Asset { get; set; }
  public virtual IList<AssetTrigger> Triggers { get; set; }
  public string Text { get; set; }

  [JsonProperty("Progress Move")]
  public bool ProgressMove { get; set; }
}

public record ConditionMeter
{
  [JsonIgnore]
  public int Id { get; set; }
  public string Name { get; set; }
  public int Max { get; set; }

  public IList<string> Conditions { get; set; }

  [JsonProperty("Starts At")]
  public int? StartsAt { get; set; }
}

public record Counter
{
  [JsonIgnore]
  public int Id { get; set; }
  [JsonProperty("Name")]
  public string Name { get; set; }

  [JsonProperty("Starts At")]
  public int StartsAt { get; set; }

  [JsonProperty("Max")]
  public int Max { get; set; }
}

public record Track
{
  [JsonIgnore]
  public int Id { get; set; }
  [JsonProperty("Name")]
  public string Name { get; set; }

  [JsonProperty("Starts At")]
  public int StartsAt { get; set; }

  [JsonProperty("Value")]
  public int Value { get; set; }
}

public record AlterMove
{
  [JsonIgnore]
  public int Id { get; set; }
  [JsonProperty("Any Move")]
  public bool AnyMove { get; set; }
  public string Name { get; set; }
  public virtual IList<AssetTrigger> Triggers { get; set; }
}

public record Select
{
  [JsonIgnore]
  public int Id { get; set; }
  public string Name { get; set; }
  public string Type { get; set; }
  public IList<string> Options { get; set; }
}