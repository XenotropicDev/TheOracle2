using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2.UserContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2Tests;
using Microsoft.Extensions.DependencyInjection;

namespace TheOracle2.UserContent.Tests
{
    [TestClass()]
    public class GameItemTests
    {
        [TestMethod()]
        public void GameItemTest()
        {
            var services = TestServices.GetServices();
            var context = services.GetRequiredService<EFContext>();
            var user = OracleGuild.GetGuild(1, context);

            Assert.IsNotNull(user);


        }
    }
}