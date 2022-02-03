using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2
{
    internal class InfoRollerResult : ITableResult
    {
        private readonly OracleInfo info;

        public string Name => info.Name;

        public string Category => info.Name;

        public string Image => null;

        public string Description => info.Description ?? info.Name;

        public InfoRollerResult(OracleInfo oracleInfo)
        {
            this.info = oracleInfo;
        }
    }

    internal class OracleInfoRoller : ITableRoller
    {
        private Random random;
        private EFContext context;
        private OracleInfo oracleInfo;

        public OracleInfoRoller(Random random, EFContext context, OracleInfo info)
        {
            this.random = random;
            this.context = context;
            this.oracleInfo = info;
        }

        public void AddStubRoll(OracleStub stub, OracleRollerResult parent)
        {
            throw new NotImplementedException();
        }

        public OracleRollerResult Build()
        {
            OracleRollerResult resultRoot = null;

            foreach (var oracle in oracleInfo.Oracles.Where(o => o.Initial))
            {
                if (resultRoot == null)
                {
                    resultRoot = new OracleRoller(random, context, oracle).Build();
                    continue;
                }

                resultRoot.ChildResults.Add(new OracleRoller(random, context, oracle).Build());
            }

            resultRoot ??= new OracleRollerResult().SetRollResult(null, new InfoRollerResult(oracleInfo));
            foreach (var oracle in oracleInfo.Oracles.Where(o => !o.Initial))
            {
                resultRoot.FollowUpTables.Add(oracle);
            }

            //todo: subcategory support?

            return resultRoot;
        }
    }
}
