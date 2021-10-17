using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TheOracle2.Tests
{
    [TestClass()]
    public class OracleInfoTests
    {
        [TestMethod()]
        public void OracleInfoTest()
        {
            var file = System.IO.File.ReadAllText("Data\\oracles.json");

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Error;

            var output = JsonConvert.DeserializeObject<List<OracleInfo>>(file, settings);

            Assert.IsTrue(output.Count > 0);
        }
    }
}