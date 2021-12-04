using System.ComponentModel.DataAnnotations.Schema;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace OracleData;

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record Ability
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }

    [Newtonsoft.Json.JsonProperty("Alter Moves", Required = Newtonsoft.Json.Required.Default)]
    public IList<AlterMove> AlterMoves { get; set; }

    [Newtonsoft.Json.JsonProperty("Alter Properties", Required = Newtonsoft.Json.Required.Default)]
    public AlterProperties AlterProperties { get; set; }

    [Newtonsoft.Json.JsonProperty("Counter")]
    public Counter Counter { get; set; }

    [Newtonsoft.Json.JsonProperty("Enabled")]
    public bool Enabled { get; set; }

    [Newtonsoft.Json.JsonProperty("Input")]
    //[NotMapped]
    public System.Collections.Generic.IList<string> Input { get; set; }

    public Move Move { get; set; }

    [Newtonsoft.Json.JsonProperty("Text")]
    public string Text { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record AlterProperties
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }
    [Newtonsoft.Json.JsonProperty("Condition Meter")]
    public ConditionMeter ConditionMeter { get; set; }

    [Newtonsoft.Json.JsonProperty("Track")]
    public Track Track { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record Asset
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public IList<OracleGuild> OracleGuilds { get; set; } = new List<OracleGuild>();

    [Newtonsoft.Json.JsonProperty("Abilities")]
    public System.Collections.Generic.IList<Ability> Abilities { get; set; }

    [Newtonsoft.Json.JsonProperty("Aliases")]
    public System.Collections.Generic.IList<string> Aliases { get; set; }

    [Newtonsoft.Json.JsonProperty("Category")]
    public string Category { get; set; }

    [Newtonsoft.Json.JsonProperty("Condition Meter")]
    public ConditionMeter ConditionMeter { get; set; }

    [Newtonsoft.Json.JsonProperty("Counter")]
    public Counter Counter { get; set; }

    [Newtonsoft.Json.JsonProperty("Description")]
    public string Description { get; set; }

    [Newtonsoft.Json.JsonProperty("Input")]
    [NotMapped]
    public System.Collections.Generic.IList<string> Input { get; set; }

    [Newtonsoft.Json.JsonProperty("Name")]
    public string Name { get; set; }

    [Newtonsoft.Json.JsonProperty("Select")]
    public Select Select { get; set; }

    [Newtonsoft.Json.JsonProperty("Track")]
    public Track Track { get; set; }
}

public record ConditionMeter
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Max { get; set; }

    [Newtonsoft.Json.JsonProperty("Starts At")]
    public int StartsAt { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record Counter
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }
    [Newtonsoft.Json.JsonProperty("Name")]
    public string Name { get; set; }

    [Newtonsoft.Json.JsonProperty("Starts At")]
    public int StartsAt { get; set; }

    [Newtonsoft.Json.JsonProperty("Max")]
    public int Max { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record Source
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }
    [Newtonsoft.Json.JsonProperty("Name")]
    public string Name { get; set; }

    [Newtonsoft.Json.JsonProperty("Page")]
    public string Page { get; set; }

    [Newtonsoft.Json.JsonProperty("Date", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Date { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record Track
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }
    [Newtonsoft.Json.JsonProperty("Name")]
    public string Name { get; set; }

    [Newtonsoft.Json.JsonProperty("Starts At", Required = Newtonsoft.Json.Required.Always)]
    public int StartsAt { get; set; }

    [Newtonsoft.Json.JsonProperty("Value", Required = Newtonsoft.Json.Required.Always)]
    public int Value { get; set; }
}

[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
public record AssetRoot
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }
    [Newtonsoft.Json.JsonProperty("Assets")]
    public System.Collections.Generic.IList<Asset> Assets { get; set; }

    [Newtonsoft.Json.JsonProperty("Name")]
    public string Name { get; set; }

    [Newtonsoft.Json.JsonProperty("Source")]
    public Source Source { get; set; }

    //Todo Get rsek to convert these to tags with IDs?
    [Newtonsoft.Json.JsonProperty("Tags")]
    public System.Collections.Generic.IList<string> Tags { get; set; }
}

public record AlterMove
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }
    [Newtonsoft.Json.JsonProperty("Any Move")]
    public bool AnyMove { get; set; }
    public string Name { get; set; }
    public IList<Trigger> Triggers { get; set; }
}

public record Select
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public IList<string> Options { get; set; }
}