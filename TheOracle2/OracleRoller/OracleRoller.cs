using System.ComponentModel;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2;

public class OracleRoller : ITableRoller
{
    private int tableId = -1;

    public OracleRoller(Random random, EFContext context, Oracle oracle)
    {
        Random = random;
        Context = context;
        Oracle = oracle;
    }

    public EFContext Context { get; }
    public Oracle Oracle { get; private set; }
    public Random Random { get; }

    public OracleRoller WithTable(int TableId)
    {
        tableId = TableId;
        return this;
    }

    public OracleRoller WithOracle(Oracle oracle)
    {
        Oracle = oracle;
        return this;
    }

    private OracleRoller MakeChild() => new OracleRoller(Random, Context, Oracle).WithTable(tableId);

    public OracleRollerResult Build()
    {
        var singleRollVal = singleRoll(Oracle, tableId);
        var resultRoot = new OracleRollerResult().SetRollResult(singleRollVal.Roll, singleRollVal.ChanceTable);
        var mainRoll = singleRollVal.ChanceTable;

        for (int i = 1; i <= mainRoll.Multiplerolls?.Amount; i++)
        {
            var rollResult = MakeChild().Build();
            resultRoot.ChildResults.Add(rollResult);
        }

        if (mainRoll.Oracles != null)
        {
            foreach (var stub in mainRoll.Oracles)
            {
                AddStubRoll(stub, resultRoot);
            }
        }

        if ((mainRoll.Oracles?.Count ?? 0) == 0 && mainRoll.Description.StartsWith("▶️"))
        {
            string[] items = mainRoll.Description.Replace("▶️", "").Split(" + ");
            foreach (string item in items)
            {
                var stub = new OracleStub { Category = Oracle.Category, Name = item };
                AddStubRoll(stub, resultRoot);
            }
        }

        return resultRoot;
    }

    public void AddStubRoll(OracleStub stub, OracleRollerResult parent)
    {
        var oracleInfo = Context.OracleInfo.FirstOrDefault(oi => oi.Name == stub.Category);

        var oracle =
            oracleInfo?.Oracles.FirstOrDefault(o => o.Name == stub.Name)
            ?? oracleInfo?.Subcategories.Select(oi => oi.Oracles.FirstOrDefault(o => o.Name == stub.Name)).FirstOrDefault();

        if (oracle != null)
            parent.ChildResults.Add(MakeChild().WithOracle(oracle).Build());

        var subcat = oracleInfo?.Subcategories?.FirstOrDefault(sub => sub.Name == stub.Name);
        if (subcat != null)
        {
            var temp = new SubcategoryRoller(Random, Context, subcat).Build();
            parent.TableResult = temp.TableResult;
            parent.ChildResults = temp.ChildResults;
            parent.FollowUpTables = temp.FollowUpTables;          
        }
    }

    private SingleRoll singleRoll(Oracle oracle, int tableId = -1)
    {
        if (oracle.Table?.Count > 0)
        {
            int roll = Random.Next(1, oracle.Table.Max(t => t.Chance) + 1);
            var table = oracle.Table.OrderBy(t => t.Chance).FirstOrDefault(t => t.Chance >= roll);
            return new SingleRoll(roll, table);
        }

        if (oracle.Tables?.Count > 0)
        {
            var reqMatch = oracle.Tables.Find(t => t.Id == tableId);
            int roll = Random.Next(1, reqMatch.Table.Max(t => t.Chance) + 1);

            var table = reqMatch.Table.OrderBy(t => t.Chance).FirstOrDefault(t => t.Chance >= roll);
            return new SingleRoll(roll, table);
        }

        return null;
    }

    private class SingleRoll
    {
        public SingleRoll(int roll, ChanceTable chanceTable)
        {
            Roll = roll;
            ChanceTable = chanceTable;
        }

        public int Roll { get; set; }
        public ChanceTable ChanceTable { get; set; }
    }
}

public interface ITableRoller
{
    OracleRollerResult Build();

    void AddStubRoll(OracleStub stub, OracleRollerResult parent);
}
