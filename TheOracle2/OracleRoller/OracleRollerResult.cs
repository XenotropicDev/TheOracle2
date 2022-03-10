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

    public bool HasResultTable(string TableName)
    {
        if (TableResult.Name == TableName) return true;
        foreach (var child in ChildResults)
        {
            if (child.HasResultTable(TableName)) return true;
        }

        return false;
    }

    public void CleanFollowupItems()
    {
        RemoveFollowUpItems(GetAllRolledTables());
    }

    public IEnumerable<string> GetAllRolledTables()
    {
        var rolledTables = new List<string>();
        rolledTables.Add(TableResult.Name);
        foreach(var child in ChildResults)
        {
            rolledTables.AddRange(child.GetAllRolledTables());
        }
        return rolledTables;
    }

    private void RemoveFollowUpItems(IEnumerable<string> tablesToRemove)
    {
        FollowUpTables.RemoveAll(t => tablesToRemove.Contains(t.Name));
        foreach(var child in ChildResults)
        {
            child.RemoveFollowUpItems(tablesToRemove);
        }
    }
}

public interface IRollResult
{
    int Chance { get; set; }
    string Description { get; set; }
    string Image { get; set; }
    public Oracle Oracle { get; set; }
}
