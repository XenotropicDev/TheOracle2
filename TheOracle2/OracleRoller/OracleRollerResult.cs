using TheOracle2.DataClasses;

namespace TheOracle2;

public class OracleRollerResult
{
    public OracleRollerResult()
    {
        ChildResults = new List<OracleRollerResult>();
        FollowUpTables = new List<Oracle>();
    }

    public ITableResult TableResult { get; internal set; }
    public int? Roll { get; internal set; }

    public OracleRollerResult SetRollResult(int? roll, ITableResult chanceTable)
    {
        TableResult = chanceTable;
        Roll = roll;

        return this;
    }

    public List<OracleRollerResult> ChildResults { get; set; }
    public List<Oracle> FollowUpTables { get; internal set; }

    internal List<OracleRollerResult> GetLastResults()
    {
        if (ChildResults == null || ChildResults.Count == 0) return new List<OracleRollerResult>() { this };

        var list = new List<OracleRollerResult>();
        foreach (var child in ChildResults)
        {
            list.AddRange(child.GetLastResults());
        }

        return list;
    }
}

public interface IRollResult
{
    int Chance { get; set; }
    string Description { get; set; }
    string Image { get; set; }
    public Oracle Oracle { get; set; }
}
