using OracleData;
using TheOracle2.UserContent;

namespace TheOracle2.DataClasses;

public class Move
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public IList<OracleGuild> OracleGuilds { get; set; } = new List<OracleGuild>();

    public string Asset { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public bool Oracle { get; set; }

    [Newtonsoft.Json.JsonProperty("Progress Move")]
    public bool IsProgressMove { get; set; }

    public string Text { get; set; }
    public List<Trigger> Triggers { get; set; }
}

public class MovesInfo
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }

    public List<Move> Moves { get; set; }
    public string Name { get; set; }
    public Source Source { get; set; }
}

public class StatOptions
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }

    public string Method { get; set; }
    public IList<string> Stats { get; set; }
}

public class Trigger
{
    [Newtonsoft.Json.JsonIgnore]
    public int Id { get; set; }

    public string Details { get; set; }

    [Newtonsoft.Json.JsonProperty("Stat Options")]
    public StatOptions StatOptions { get; set; }

    public string Text { get; set; }
}