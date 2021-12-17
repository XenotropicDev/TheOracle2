using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
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