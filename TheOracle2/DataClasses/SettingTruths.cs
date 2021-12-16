using OracleData;

namespace TheOracle2.DataClasses;

//Todo convert the classes in the file to records that should be

public partial class TruthRoot
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Source")]
    public Source Source { get; set; }

    [JsonPropertyName("Setting Truths")]
    public IList<SettingTruth> SettingTruths { get; set; }
}

public partial class SettingTruth
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Table")]
    public IList<SettingTruthTable> Table { get; set; }

    [JsonPropertyName("Character")]
    public string Character { get; set; }
}

public partial class SettingTruthTable
{
    [JsonPropertyName("Chance")]
    public long Chance { get; set; }

    [JsonPropertyName("Description")]
    public string Description { get; set; }

    [JsonPropertyName("Details")]
    public string Details { get; set; }

    [JsonPropertyName("Table")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IList<ChanceDescriptionStub> Table { get; set; }

    [JsonPropertyName("Quest Starter")]
    public string QuestStarter { get; set; }
}

public partial class ChanceDescriptionStub
{
    [JsonPropertyName("Chance")]
    public long Chance { get; set; }

    [JsonPropertyName("Description")]
    public string Description { get; set; }
}
