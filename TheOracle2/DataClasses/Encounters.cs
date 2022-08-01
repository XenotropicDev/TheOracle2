namespace TheOracle2.DataClasses;

public class Variant
{
    [JsonProperty("$id")]
    public string Id { get; set; }
    public Source Source { get; set; }
    public string Name { get; set; }
    public int Rank { get; set; }
    public Display Display { get; set; }
    public string Description { get; set; }
    public string Nature { get; set; }

    [JsonProperty("Variant of")]
    public string VariantOf { get; set; }
    public List<string> Tags { get; set; }
}

public class EncounterRoot
{
    [JsonProperty("$id")]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Nature { get; set; }
    public string Summary { get; set; }
    public int Rank { get; set; }
    public Display Display { get; set; }
    public List<string> Features { get; set; }
    public List<string> Drives { get; set; }
    public List<string> Tactics { get; set; }
    public string Description { get; set; }

    [JsonProperty("Quest Starter")]
    public string QuestStarter { get; set; }
    public Source Source { get; set; }
    public List<Variant> Variants { get; set; }
}

