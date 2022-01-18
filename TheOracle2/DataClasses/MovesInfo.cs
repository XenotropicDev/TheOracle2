using TheOracle2.UserContent;

namespace TheOracle2.DataClasses;

public class Move
{
  public string Id { get; set; }
  [JsonIgnore]
  public virtual IList<OracleGuild> OracleGuilds { get; set; }

  public string Category { get; set; }
  public string Name { get; set; }
  public bool Oracle { get; set; }

  [JsonProperty("Progress Move")]
  public bool IsProgressMove { get; set; }

  [JsonProperty("Trigger text")]
  public string TriggerText { get; set; }
  public string Text { get; set; }
  public virtual List<MoveTrigger> Triggers { get; set; }
}

public class MovesInfo
{
  [JsonIgnore]
  public int Id { get; set; }

  public virtual List<Move> Moves { get; set; }
  public string Name { get; set; }
  public virtual Source Source { get; set; }
}

public class MoveStatOptions
{
  [JsonIgnore]
  public int Id { get; set; }

  public string Method { get; set; }
  public IList<string> Stats { get; set; }

  public IList<string> Progress { get; set; }

  public IList<string> Legacies { get; set; }
}

public class MoveTrigger
{
  [JsonIgnore]
  public int Id { get; set; }

  public string Details { get; set; }

  [JsonProperty("Stat Options")]
  public virtual MoveStatOptions StatOptions { get; set; }

  public string Text { get; set; }
  public virtual IList<Special> Special { get; set; }
}