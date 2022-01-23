using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2
{
    internal class SubcategoryTableResult : ITableResult
    {
        private readonly Subcategory subcategory;

        public string Name => subcategory.Name;

        public string Category => subcategory.Category;

        public string Image => subcategory.Image;

        public string Description => subcategory.Description;

        public SubcategoryTableResult(Subcategory subcategory)
        {
            this.subcategory = subcategory;
        }
    }

    internal class SubcategoryRoller : ITableRoller
    {
        private Random random;
        private EFContext context;
        private Subcategory subcat;

        public SubcategoryRoller(Random random, EFContext context, Subcategory subcat)
        {
            this.random = random;
            this.context = context;
            this.subcat = subcat;
        }

        public void AddStubRoll(OracleStub stub, OracleRollerResult parent)
        {
            throw new NotImplementedException();
        }

        public OracleRollerResult Build()
        {
            var resultRoot = new OracleRollerResult().SetRollResult(null, new SubcategoryTableResult(subcat));

            foreach (var oracle in subcat.Oracles)
            {
                if (oracle.Initial) resultRoot.ChildResults.Add(new OracleRoller(random, context, oracle).Build());
                else resultRoot.FollowUpTables.Add(oracle);
            }

            return resultRoot;
        }
    }
}
