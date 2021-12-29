using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2Tests;
using Microsoft.Extensions.DependencyInjection;
using TheOracle2.UserContent;

namespace TheOracle2.Tests
{
    [TestClass()]
    public class OracleCommandTests
    {
        [TestMethod()]
        public void GetCommandBuildersTest()
        {
            var services = TestServices.GetServices();
            var context = services.GetRequiredService<EFContext>();
            var random = services.GetRequiredService<Random>();
            var cmd = new OracleCommand(context, random);

            //cmd.GetCommandBuilders();
        }
    }
}