namespace TheOracle2.DataClasses;

public class Source
{
    public string Title { get; set; }
    public string Date { get; set; }
    public int Page { get; set; }
}

public class Display
{
    public string Title { get; set; }
    public string Color { get; set; }
    public string Icon { get; set; }
    public List<string> Images { get; set; }
}

public class MoveOutcome
{
    [JsonProperty("$id")]
    public string Id { get; set; }
    public string Text { get; set; }

    [JsonProperty("With a Match")]
    public MoveOutcome WithAMatch { get; set; }
}

public class Outcomes
{
    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Strong Hit")]
    public MoveOutcome StrongHit { get; set; }

    [JsonProperty("Weak Hit")]
    public MoveOutcome WeakHit { get; set; }
    public MoveOutcome Miss { get; set; }
}

public class Usage
{
    [JsonProperty("Max rolls")]
    public int MaxRolls { get; set; }

    [JsonProperty("Allow duplicates")]
    public bool AllowDuplicates { get; set; }
    public bool? Initial { get; set; }
    public Suggestions Suggestions { get; set; }

    [JsonProperty("Sets attributes")]
    public List<SetsAttribute> SetsAttributes { get; set; }
    public Requires Requires { get; set; }
    public bool? Repeatable { get; set; }
    public bool Shared { get; set; }

}
public class CustomStat
{
    [JsonProperty("$id")]
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Option> Options { get; set; }
}

public class Option
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Set> Set { get; set; }
    public string Text { get; set; }

    [JsonProperty("Roll type")]
    public string RollType { get; set; }
    public string Method { get; set; }
    public List<string> Using { get; set; }
    public int Value { get; set; }

    [JsonProperty("Custom stat")]
    public CustomStat CustomStat { get; set; }
}
