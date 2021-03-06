using System.Text.RegularExpressions;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2.ActionRoller
{
    public class TableRollerFactory
    {
        public TableRollerFactory(EFContext context, Random random)
        {
            Context = context;
            Random = random;
        }

        private EFContext Context { get; }
        private Random Random { get; }

        public ITableRoller GetRoller(Oracle oracle)
        {
            return new OracleRoller(Random, Context, oracle);
        }

        public ITableRoller GetRoller(Tables table)
        {
            return new OracleRoller(Random, Context, table.Oracle).WithTable(table.Id);
        }

        public ITableRoller GetRoller(string value, bool strict = false)
        {
            if (!int.TryParse(value.AsSpan(value.IndexOf(":") + 1), out int id))
            {
                var matches = Context.Oracles.Where(o => Regex.IsMatch(o.Name, $@"\b(?i){value}"));

                if (matches.Count() != 1) throw new ArgumentException($"Unknown id {value}");

                id = matches.FirstOrDefault().Id;
                value = $"oracle:{id}";
            }

            if (value.StartsWith("tables:"))
            {
                var table = Context.Tables.Find(id);
                return new OracleRoller(Random, Context, table.Oracle).WithTable(table.Id);
            }

            if (value.StartsWith("subcat:"))
            {
                var subcat = Context.Subcategory.Find(id);
                return new SubcategoryRoller(Random, Context, subcat);
            }

            if (value.StartsWith("info:"))
            {
                var info = Context.OracleInfo.Find(id);
                return new OracleInfoRoller(Random, Context, info);
            }

            if (!strict || value.StartsWith("oracle:"))
            {
                var oracle = Context.Oracles.Find(id);
                return new OracleRoller(Random, Context, oracle);
            }

            return null;
        }
    }
}
