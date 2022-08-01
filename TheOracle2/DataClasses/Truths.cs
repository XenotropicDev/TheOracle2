namespace TheOracle2.DataClasses;

public class TruthTable
{
    public int Floor { get; set; }
    public int Ceiling { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }
    public string Result { get; set; }
    public List<TruthTable> Subtable { get; set; }
    public string Description { get; set; }

    [JsonProperty("Quest Starter")]
    public string QuestStarter { get; set; }

    [JsonProperty("Roll template")]
    public RollTemplate RollTemplate { get; set; }
}

public class TruthsRoot
{
    [JsonProperty("$id")]
    public string Id { get; set; }
    public string Name { get; set; }
    public List<TruthTable> Table { get; set; }
    public string Character { get; set; }
    public Suggestions Suggestions { get; set; }
    public Source Source { get; set; }
}

