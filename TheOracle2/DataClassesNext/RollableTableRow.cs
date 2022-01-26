using System.ComponentModel;

namespace TheOracle2.DataClassesNext;

public class RollableTableRow
{
    public bool RollIsInRange(int roll)
    {
        if (roll >= Floor && roll <= Ceiling) { return true; }
        return false;
    }

    public override string ToString()
    {
        return $"`{ToRangeString().PadLeft(7)}` {ToResultString()}";
    }

    public string ToResultString()
    {
        if (Summary != null)
        {
            return $"{Result} ({Summary})";
        }
        return Result;
    }

    public string ToRangeString()
    {
        if (Floor == Ceiling) { return Floor.ToString(); }
        return $"{Floor}-{Ceiling}";
    }

    public int Floor { get; set; }

    public int Ceiling { get; set; }

    public string Result { get; set; }

    public string Summary { get; set; }

    [JsonProperty("Game objects")]
    public GameObject[] GameObjects { get; set; }

    [JsonProperty("Multiple rolls")]
    public MultipleRolls MultipleRolls { get; set; }

    [JsonProperty("Oracle rolls")]
    public string[] OracleRolls { get; set; }

    public Suggestions Suggestions { get; set; }

    [DefaultValue(null)]
    public int Amount { get; set; }

    [JsonProperty("Part of speech")]
    public IList<string> PartOfSpeech { get; set; }

    public IList<string> Assets { get; set; }

    public RollableTable Table { get; set; }

    [JsonProperty("Add template")]
    public AddTemplate AddTemplate { get; set; }

    public string Image { get; set; }
}
