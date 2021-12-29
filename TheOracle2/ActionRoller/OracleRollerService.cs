using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2;

public class OracleRollerService : IRollerService
{
    public OracleRollerService(Random random, EFContext context)
    {
        Random = random;
        Context = context;
    }

    public EFContext Context { get; }
    public Random Random { get; }

    public OracleRollerResult Roll(Oracle oracle, Requires requires = null)
    {
        var mainRoll = singleRoll(oracle, requires);
        var resultRoot = new OracleRollerResult { Result = mainRoll };

        for (int i = 1; i <= mainRoll.Multiplerolls?.Amount; i++)
        {
            var rollResult = Roll(oracle, requires);
            resultRoot.ChildResults.Add(rollResult);
        }

        if (mainRoll.Oracles != null)
        {
            foreach (var stub in mainRoll.Oracles)
            {
                resultRoot.ChildResults.Add(RollStub(stub, requires));
            }
        }

        if ((mainRoll.Oracles?.Count ?? 0) == 0 && mainRoll.Description.StartsWith("▶️"))
        {
            string[] items = mainRoll.Description.Replace("▶️", "").Split(" + ");
            foreach (string item in items)
            {
                var stub = new OracleStub { Category = oracle.Category, Name = item };
                resultRoot.ChildResults.Add(RollStub(stub, requires));
            }
        }

        return resultRoot;
    }

    public OracleRollerResult RollStub(OracleStub stub, Requires requires)
    {
        var oracleInfo = Context.OracleInfo.FirstOrDefault(oi => oi.Name == stub.Category);

        var oracle =
            oracleInfo?.Oracles.FirstOrDefault(o => o.Name == stub.Name)
            ?? oracleInfo?.Subcategories.Select(oi => oi.Oracles.FirstOrDefault(o => o.Name == stub.Name)).FirstOrDefault();

        if (oracle != null)
            return Roll(oracle, requires);

        var test = Context.OracleInfo.FirstOrDefault(oi => oi.Name == stub.Category);
        var baseResult = new OracleRollerResult { Result = new ChanceTable { Description = stub.Name } };

        var tables = oracleInfo?.Subcategories?.FirstOrDefault(sub => sub.Name == stub.Name)?.Oracles;
        if (tables != null)
        {
            foreach (var table in tables)
            {
                if (table.Initial)
                {
                    var subroll = Roll(table, requires);
                    if (baseResult.Result.Oracle == null)
                    {
                        baseResult.Result.Oracle = table;
                        baseResult.Result.OracleId = table.Id;
                    }
                    baseResult.ChildResults.Add(subroll);
                }
                else baseResult.FollowUpTables.Add(table);
            }
        }

        return baseResult;
    }

    private ChanceTable singleRoll(Oracle oracle, Requires requires = null)
    {
        int roll = Random.Next(1, oracle.Table.Max(t => t.Chance) + 1);

        if (oracle.Table?.Count > 0)
        {
            return oracle.Table.OrderBy(t => t.Chance).FirstOrDefault(t => t.Chance >= roll);
        }

        if (oracle.Tables?.Count > 0)
        {
            var reqMatch = oracle.Tables.Single(t => t.Requires == requires);
            return reqMatch.Table.OrderBy(t => t.Chance).FirstOrDefault(t => t.Chance >= roll);
        }

        return null;
    }
}

public interface IRollerService
{
    OracleRollerResult Roll(Oracle oracle, Requires requires = null);

    OracleRollerResult RollStub(OracleStub stub, Requires requires);
}