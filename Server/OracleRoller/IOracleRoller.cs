using Server.Data;
using TheOracle2;
using TheOracle2.Data;

namespace Server.OracleRoller
{
    public interface IOracleRoller
    {
        OracleRollResult GetRollResult(Oracle oracle);
    }

    public class RandomOracleRoller : IOracleRoller
    {
        private readonly Random random;
        private readonly IOracleRepository oracleRepo;

        public RandomOracleRoller(Random random, IOracleRepository oracleRepo)
        {
            this.random = random;
            this.oracleRepo = oracleRepo;
        }

        public OracleRollResult GetRollResult(Oracle oracle)
        {
            var results = new OracleRollResult();
            results.Oracle = oracle;
            foreach (var followUpId in oracle.Usage?.Suggestions?.OracleRolls ?? new List<string>())
            {
                var followUpOracle = oracleRepo.GetOracle(followUpId);
                if (followUpOracle?.Oracles?.Count > 0)
                {
                    foreach (var subTable in followUpOracle.Oracles)
                    {
                        results.FollowUpTables.Add(new FollowUpItem(subTable.Id, subTable.Name));
                    }
                }
                else
                {
                    results.FollowUpTables.Add(new FollowUpItem(followUpId, followUpOracle.Name));
                }
            }

            if (oracle.Table?.Count > 0)
            {
                var roll = random.Next(1, 101);
                var tableItem = oracle.Table.FirstOrDefault(t => t.CompareTo(roll) == 0);

                if (tableItem != null)
                {
                    results.WithTableResult(tableItem, roll);
                }
            }

            foreach (var subOracle in oracle.Oracles ?? new List<Oracle>())
            {
                results.ChildResults.Add(GetRollResult(subOracle));
            }

            return results;
        }
    }

    public class RandomTableRoller : IOracleRoller
    {
        private readonly Random random;

        public RandomTableRoller(Random random)
        {
            this.random = random;
        }

        public OracleRollResult GetRollResult(Oracle oracle)
        {
            throw new NotImplementedException();
        }
    }
}
