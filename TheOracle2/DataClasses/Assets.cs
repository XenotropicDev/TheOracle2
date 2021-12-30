using System.ComponentModel.DataAnnotations.Schema;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace OracleData;

public record Ability
{
    [JsonIgnore]
    public int Id { get; set; }

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

    public virtual Move Move { get; set; }

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

public record Asset
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public virtual IList<OracleGuild> OracleGuilds { get; set; }

    [JsonProperty("Abilities")]
    public virtual IList<Ability> Abilities { get; set; }

    [JsonProperty("Aliases")]
    public IList<string> Aliases { get; set; }

    [JsonProperty("Category")]
    public string Category { get; set; }

    [JsonProperty("Condition Meter")]
    public virtual ConditionMeter ConditionMeter { get; set; }

    [JsonProperty("Counter")]
    public virtual Counter Counter { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Input")]
    [NotMapped]
    public IList<string> Input { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Select")]
    public virtual Select Select { get; set; }

    [JsonProperty("Track")]
    public virtual Track Track { get; set; }
}

public record ConditionMeter
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Max { get; set; }

    [JsonProperty("Starts At")]
    public int StartsAt { get; set; }
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

public record Source
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Page")]
    public string Page { get; set; }

    [JsonProperty("Date", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Date { get; set; }
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

    //Todo Get rsek to convert these to tags with IDs?
    [JsonProperty("Tags")]
    public IList<string> Tags { get; set; }
}

public record AlterMove
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonProperty("Any Move")]
    public bool AnyMove { get; set; }
    public string Name { get; set; }
    public virtual IList<Trigger> Triggers { get; set; }
}

public record Select
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public IList<string> Options { get; set; }
}