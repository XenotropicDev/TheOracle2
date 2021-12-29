using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle2Tests;

namespace TheOracle2.UserContent.Tests
{
    [TestClass()]
    public class GameItemTests
    {
        [TestMethod()]
        public void EFContextTest()
        {
            var services = TestServices.GetServices();
            var context = services.GetRequiredService<EFContext>();
            var user = OracleGuild.GetGuild(1, context);

            Assert.IsNotNull(user);
        }

        [TestMethod()]
        public async Task EFContextLinkingTest()
        {
            var services = TestServices.GetServices();
            var context = services.GetRequiredService<EFContext>();

            Assert.IsTrue(context.Assets.All(a => a.Abilities.Count > 0));
        }

        [TestMethod()]
        public async Task OracleRollerTest()
        {
            var services = TestServices.GetServices();
            var context = services.GetRequiredService<EFContext>();

            //var result = context.Oracles.Find(1).Roll(services.GetRequiredService<Random>());

            //Assert.IsTrue(context.Assets.All(a => a.Abilities.Count > 0));
            
            //var displayNames = context.OracleInfo.Select(oi => oi.DisplayName).ToList();
            //Console.WriteLine($"Oracle Categories:\n{string.Join("\n", displayNames)}");

            //var oiWithSub = context.OracleInfo.Where(oi => oi.Subcategories.Count > 0);
            //var subcatDisplay = oiWithSub.SelectMany(oi => oi.Subcategories).Select(sc => sc.Displayname).ToList();
            //Console.WriteLine("Sub Categories:\n" + String.Join("\n", subcatDisplay));
        }
    }

    [TestClass()]
    public class RegexTest
    {
        [TestMethod()]
        public void regex()
        {
            string expected = "replace these 'But  not these' and replace these";

            string source = "replace      these    'But  not these' and    replace  these";
            var regex = new Regex(@"\s+");

            var result = regex.Replace(source, " ");

            Assert.AreEqual(expected, result);
        }
    }
}