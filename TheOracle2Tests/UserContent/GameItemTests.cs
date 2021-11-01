using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
}