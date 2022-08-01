namespace TheOracle2.DataClasses;

public class Trigger
{
    [JsonProperty("$id")]
    public string Id { get; set; }
    public string Text { get; set; }
    public List<Option> Options { get; set; }
    public By By { get; set; }
}

public class By
{
    public bool Player { get; set; }
    public bool Ally { get; set; }
}


public class MoveRoot
{
    [JsonProperty("$id")]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Source Source { get; set; }
    public Display Display { get; set; }
    public List<Move> Moves { get; set; }
}

public class Move
{
    public Source Source { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string Asset { get; set; }
    public Display Display { get; set; }
    public Trigger Trigger { get; set; }
    public string Text { get; set; }
    public Outcomes Outcomes { get; set; }

    [JsonProperty("Progress Move")]
    public bool? ProgressMove { get; set; }

    public List<string> Oracles { get; set; }

    [JsonProperty("Variant of")]
    public string VariantOf { get; set; }
}
