using Discord.Interactions;
using Discord.WebSocket;
using TheOracle2.DataClassesNext;

namespace TheOracle2;

public class OracleDumpCommand : InteractionModuleBase
{
  public Dictionary<string, OracleNext> Oracles { get; }

  public IList<OracleCategory> StructuredOracles { get; }

  public OracleDumpCommand()
  {
    var baseDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Data"));
    var file = baseDir.GetFiles("oracle_paths.json").FirstOrDefault();
    var text = file.OpenText().ReadToEnd();
    Oracles = JsonConvert.DeserializeObject<Dictionary<string, OracleNext>>(text);
    file = baseDir.GetFiles("oracles_next.json").FirstOrDefault();
    text = file.OpenText().ReadToEnd();
    StructuredOracles = JsonConvert.DeserializeObject<IList<OracleCategory>>(text);
  }
  [SlashCommand("oracle-dump", "Dumps oracle data by path.")]
  public async Task OracleDump(
    [Summary(description: "The oracle path to dump.")]
    string key
  )
  {
    if (Oracles.ContainsKey(key))
    {
      OracleNext oracle = Oracles[key];
      string[] oraclePathParts = key.Split(" / ");
      string oraclePath = String.Join(" / ", oraclePathParts.Take(oraclePathParts.Length - 1));
      await RespondAsync(embed: oracle.ToEmbed().Build()).ConfigureAwait(false);
    }
    else
    {
      await RespondAsync($"Oracle `{key}` not found.");
    }
  }
}