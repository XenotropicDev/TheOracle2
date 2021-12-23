using OracleData;

namespace TheOracle2.DataClasses;

public partial class EncountersRoot
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Source")]
    public virtual Source Source { get; set; }

    [JsonPropertyName("Encounters")]
    public virtual IList<Encounter> Encounters { get; set; }
}

public partial class Encounter
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Rank")]
    public long Rank { get; set; }

    [JsonPropertyName("Features")]
    public IList<string> Features { get; set; }

    [JsonPropertyName("Drives")]
    public IList<string> Drives { get; set; }

    [JsonPropertyName("Tactics")]
    public IList<string> Tactics { get; set; }

    [JsonPropertyName("Variants")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual IList<Variant> Variants { get; set; }

    [JsonPropertyName("Description")]
    public string Description { get; set; }

    [JsonPropertyName("Quest Starter")]
    public string QuestStarter { get; set; }
}

public partial class Variant
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Rank")]
    public long Rank { get; set; }

    [JsonPropertyName("Description")]
    public string Description { get; set; }
}