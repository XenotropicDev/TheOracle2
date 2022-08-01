namespace TheOracle2.DataClasses;

public class Attachments
{
    [JsonProperty("Asset Types")]
    public List<string> AssetTypes { get; set; }
    public object Max { get; set; }
}

public class Set
{
    public string Key { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }

}

public class Input
{
    public string Id { get; set; }
    public string Name { get; set; }

    [JsonProperty("Input Type")]
    public string InputType { get; set; }
    public bool Adjustable { get; set; }
    public List<Set> Sets { get; set; }
    public List<Option> Options { get; set; }
    public int Step { get; set; }
    public int Min { get; set; }
    public int Value { get; set; }

    [JsonProperty("Clock Type")]
    public string ClockType { get; set; }
    public int? Segments { get; set; }
    public int? Filled { get; set; }
}

public class ConditionMeter
{
    public int Min { get; set; }
    public int Value { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public int Max { get; set; }
    public List<string> Conditions { get; set; }
    public List<string> Aliases { get; set; }
}



public class AlterMove
{
    public string Id { get; set; }
    public List<string> Moves { get; set; }
    public Trigger Trigger { get; set; }
    public List<string> Alters { get; set; }
}


public class Effect
{
    public string Text { get; set; }
}

public class Burn
{
    public Trigger Trigger { get; set; }
    public Effect Effect { get; set; }
    public string Outcome { get; set; }
}

public class Reset
{
    public Trigger Trigger { get; set; }
    public int Value { get; set; }
}

public class AlterMomentum
{
    public List<Burn> Burn { get; set; }
    public List<Reset> Reset { get; set; }
}

public class State
{
    public string Name { get; set; }
    public bool Enabled { get; set; }

    [JsonProperty("Disables asset")]
    public bool DisablesAsset { get; set; }
}

public class AlterProperties
{
    public Attachments Attachments { get; set; }
    public List<State> States { get; set; }

    [JsonProperty("Condition Meter")]
    public ConditionMeter ConditionMeter { get; set; }
}

public class Ability
{
    public string Id { get; set; }
    public string Text { get; set; }
    public bool Enabled { get; set; }

    [JsonProperty("Alter Moves")]
    public List<AlterMove> AlterMoves { get; set; }
    public List<Move> Moves { get; set; }

    [JsonProperty("Alter Momentum")]
    public AlterMomentum AlterMomentum { get; set; }

    [JsonProperty("Alter Properties")]
    public AlterProperties AlterProperties { get; set; }
    public List<Input> Inputs { get; set; }
}

public class Asset
{
    public Source Source { get; set; }

    [JsonProperty("Asset Type")]
    public string AssetType { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public Display Display { get; set; }
    public Usage Usage { get; set; }
    public Attachments Attachments { get; set; }
    public List<Input> Inputs { get; set; }

    [JsonProperty("Condition Meter")]
    public ConditionMeter ConditionMeter { get; set; }
    public List<Ability> Abilities { get; set; }
    public List<State> States { get; set; }
    public string Requirement { get; set; }
    public List<string> Aliases { get; set; }
}

public class AssetRoot
{
    public Source Source { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Display Display { get; set; }
    public List<Asset> Assets { get; set; }
}

