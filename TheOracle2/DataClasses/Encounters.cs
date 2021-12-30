using OracleData;

namespace TheOracle2.DataClasses;

public partial class EncountersRoot
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Source")]
    public virtual Source Source { get; set; }

    [JsonProperty("Encounters")]
    public virtual IList<Encounter> Encounters { get; set; }
}

public partial class Encounter
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Rank")]
    public long Rank { get; set; }

    public string Nature { get; set; }

    [JsonProperty("Features")]
    public IList<string> Features { get; set; }

    [JsonProperty("Drives")]
    public IList<string> Drives { get; set; }

    [JsonProperty("Tactics")]
    public IList<string> Tactics { get; set; }

    [JsonProperty("Variants", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual IList<Variant> Variants { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Quest Starter")]
    public string QuestStarter { get; set; }
}

public partial class Variant
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Rank")]
    public long Rank { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }
}