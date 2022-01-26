namespace TheOracle2.DataClasses;

public partial class Encounter
{
    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Drives")]
    public IList<string> Drives { get; set; }

    [JsonProperty("Features")]
    public IList<string> Features { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    public string Nature { get; set; }

    [JsonProperty("Quest Starter")]
    public string QuestStarter { get; set; }

    [JsonProperty("Rank")]
    public string Rank { get; set; }

    public Source Source { get; set; }

    public string Summary { get; set; }

    [JsonProperty("Tactics")]
    public IList<string> Tactics { get; set; }

    [JsonProperty("Variants", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual IList<Variant> Variants { get; set; }
}

public partial class EncountersRoot
{
    [JsonProperty("Encounters")]
    public virtual IList<Encounter> Encounters { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Source")]
    public virtual Source Source { get; set; }
}

public partial class Variant
{
    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Rank")]
    public string Rank { get; set; }
}
