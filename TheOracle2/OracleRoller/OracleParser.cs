using TheOracle2.UserContent;

namespace TheOracle2.OracleRoller;

public class OracleParser
{
    public OracleParser(IServiceProvider services)
    {
        Services = services;
    }

    public IServiceProvider Services { get; }

    /* Things to support:
     * 
     * 
     * 
     * 
     * 
     */
    public OracleRollResult GetOracleResults(IEnumerable<int> listOfTablesToRoll)
    {
        var context = Services.GetRequiredService<EFContext>();

        foreach (var id in listOfTablesToRoll)
        {
            var oracle = context.Oracles.Find(id);
            DataClasses.Tables chanceTable;

            if (oracle == default)
            {
                chanceTable = context.ChanceTables.Find(id);
                if (chanceTable == default) throw new KeyNotFoundException($"Couldn't find key {id} in oracle data");
            }

        }

        return null;
    }
}

public class OracleRollResult
{

}
