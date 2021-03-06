namespace TheOracle2.DataClasses;

//Todo convert the classes in the file to records that should be

public partial class TruthRoot
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Source")]
    public virtual Source Source { get; set; }

    [JsonProperty("Setting Truths")]
    public virtual IList<SettingTruth> SettingTruths { get; set; }
}

public partial class SettingTruth
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Table")]
    public virtual IList<SettingTruthTable> Table { get; set; }

    [JsonProperty("Character")]
    public string Character { get; set; }
}

public partial class SettingTruthTable
{
    [JsonProperty("Chance")]
    public long Chance { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Details")]
    public string Details { get; set; }

    [JsonProperty("Table", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual IList<ChanceDescriptionStub> Table { get; set; }

    [JsonProperty("Quest Starter")]
    public string QuestStarter { get; set; }
}

public partial class ChanceDescriptionStub
{
    [JsonProperty("Chance")]
    public long Chance { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }
}
