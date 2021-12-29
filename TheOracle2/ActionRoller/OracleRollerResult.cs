using TheOracle2.DataClasses;

namespace TheOracle2;

public class OracleRollerResult
{
    public OracleRollerResult()
    {
        ChildResults = new List<OracleRollerResult>();
        FollowUpTables = new List<Oracle>();
    }

    public ChanceTable Result { get; set; }
    public List<OracleRollerResult> ChildResults { get; set; }
    public List<Oracle> FollowUpTables { get; internal set; }
}

public interface IRollResult
{
    int Chance { get; set; }
    string Description { get; set; }
    string Thumbnail { get; set; }
    public Oracle Oracle { get; set; }
}
