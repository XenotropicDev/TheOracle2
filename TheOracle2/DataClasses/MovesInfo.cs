using OracleData;
using TheOracle2.UserContent;

namespace TheOracle2.DataClasses;

public class Move
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public virtual IList<OracleGuild> OracleGuilds { get; set; }

    public string Asset { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public bool Oracle { get; set; }

    [JsonPropertyName("Progress Move")]
    public bool IsProgressMove { get; set; }

    public string Text { get; set; }
    public virtual List<Trigger> Triggers { get; set; }
}

public class MovesInfo
{
    [JsonIgnore]
    public int Id { get; set; }

    public virtual List<Move> Moves { get; set; }
    public string Name { get; set; }
    public virtual Source Source { get; set; }
}

public class StatOptions
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Method { get; set; }
    public IList<string> Stats { get; set; }
}

public class Trigger
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Details { get; set; }

    [JsonPropertyName("Stat Options")]
    public virtual StatOptions StatOptions { get; set; }

    public string Text { get; set; }
}