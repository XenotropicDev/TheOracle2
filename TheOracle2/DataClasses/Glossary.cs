namespace TheOracle2.DataClasses;

public partial class GlossaryRoot
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Category")]
    public string Category { get; set; }

    [JsonProperty("Source")]
    public virtual Source Source { get; set; }

    [JsonProperty("Terms")]
    public virtual IList<Term> Terms { get; set; }

    [JsonProperty("Description", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Description { get; set; }
}

public partial class Term
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Color", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Color { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Category")]
    public string Category { get; set; }

    [JsonProperty("Applied by", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual AppliedBy AppliedBy { get; set; }

    [JsonProperty("Removed by", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual RemovedBy RemovedBy { get; set; }

    [JsonProperty("Effects", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual Effect Effects { get; set; }

    [JsonProperty("Applies to", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public IList<string> AppliesTo { get; set; }

    [JsonProperty("Effect", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual Effect Effect { get; set; }
}

public partial class AppliedBy
{
    [JsonProperty("Moves")]
    public IList<string> Moves { get; set; }
}

public partial class Effect
{
    [JsonProperty("Forbid Increase")]
    public IList<string> ForbidIncrease { get; set; }
}

public partial class RemovedBy
{
    [JsonProperty("Moves", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public IList<string> Moves { get; set; }

    [JsonProperty("Quest", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual Quest Quest { get; set; }
}

public partial class Quest
{
    [JsonProperty("Rank")]
    public IList<string> Rank { get; set; }
}
