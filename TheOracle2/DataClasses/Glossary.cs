using OracleData;

namespace TheOracle2.DataClasses;

public partial class GlossaryRoot
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Category")]
    public string Category { get; set; }

    [JsonPropertyName("Source")]
    public virtual Source Source { get; set; }

    [JsonPropertyName("Terms")]
    public virtual IList<Term> Terms { get; set; }

    [JsonPropertyName("Description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Description { get; set; }
}

public partial class Term
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Color")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Color { get; set; }

    [JsonPropertyName("Description")]
    public string Description { get; set; }

    [JsonPropertyName("Category")]
    public string Category { get; set; }

    [JsonPropertyName("Applied by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual AppliedBy AppliedBy { get; set; }

    [JsonPropertyName("Removed by")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual RemovedBy RemovedBy { get; set; }

    [JsonPropertyName("Effects")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual Effect Effects { get; set; }

    [JsonPropertyName("Applies to")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IList<string> AppliesTo { get; set; }

    [JsonPropertyName("Effect")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual Effect Effect { get; set; }
}

public partial class AppliedBy
{
    [JsonPropertyName("Moves")]
    public IList<string> Moves { get; set; }
}

public partial class Effect
{
    [JsonPropertyName("Forbid Increase")]
    public IList<string> ForbidIncrease { get; set; }
}

public partial class RemovedBy
{
    [JsonPropertyName("Moves")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IList<string> Moves { get; set; }

    [JsonPropertyName("Quest")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual Quest Quest { get; set; }
}

public partial class Quest
{
    [JsonPropertyName("Rank")]
    public IList<string> Rank { get; set; }
}