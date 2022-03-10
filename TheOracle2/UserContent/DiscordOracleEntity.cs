using TheOracle2.ActionRoller;
using TheOracle2.DataClasses;

namespace TheOracle2.UserContent
{
    internal class DiscordOracleEntity : IDiscordEntity
    {
        private DiscordOracleItems ob;

        public DiscordOracleEntity(Oracle oracle, EFContext dbContext, Random random)
        {
            var RollerFactory = new TableRollerFactory(dbContext, random);

            var rollResult = RollerFactory.GetRoller(oracle).Build();

            ob = new DiscordOracleBuilder(rollResult).Build();
        }

        public DiscordOracleEntity(string oracleQuery, EFContext dbContext, Random random)
        {
            var RollerFactory = new TableRollerFactory(dbContext, random);

            var rollResult = RollerFactory.GetRoller(oracleQuery).Build();

            ob = new DiscordOracleBuilder(rollResult).Build();
        }

        public DiscordOracleEntity(List<string> oracleQueryList, EFContext dbContext, Random random)
        {
            var RollerFactory = new TableRollerFactory(dbContext, random);

            OracleRollerResult rollResult = new OracleRollerResult();

            foreach (string oracleQuery in oracleQueryList)
            {
                if (rollResult.TableResult == default) rollResult = RollerFactory.GetRoller(oracleQuery).Build();
                else rollResult.ChildResults.Add(RollerFactory.GetRoller(oracleQuery).Build());
            }
            rollResult.CleanFollowupItems(); //Because we hacked in the child result we need to re-clean the followup results

            ob = new DiscordOracleBuilder(rollResult).Build();
        }

        public bool IsEphemeral { get; set; } = false;

        public MessageComponent GetComponents()
        {
            return ob.ComponentBuilder.Build();
        }

        public Embed[] GetEmbeds()
        {
            return new Embed[] { ob.EmbedBuilder.Build() };
        }
    }
}
