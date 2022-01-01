using TheOracle2.DataClasses;

namespace TheOracle2;

public class OracleRollerResult
{
    public OracleRollerResult()
    {
        ChildResults = new List<OracleRollerResult>();
        FollowUpTables = new List<Oracle>();
    }

    public string Name { get => Result.Oracle?.Name ?? Result.Tables?.Displayname; }
    public string Category { get => Result.Oracle?.Category; }

    //todo: does this also need the roll value?
    public ChanceTable Result { get; private set; }
    public int Roll { get; private set; }

    public OracleRollerResult SetRollResult(int roll, ChanceTable chanceTable)
    {
        Result = chanceTable;
        Roll = roll;

        return this;
    }

    public List<OracleRollerResult> ChildResults { get; set; }
    public List<Oracle> FollowUpTables { get; internal set; }
}

public interface IRollResult
{
    int Chance { get; set; }
    string Description { get; set; }
    string Image { get; set; }
    public Oracle Oracle { get; set; }
}
