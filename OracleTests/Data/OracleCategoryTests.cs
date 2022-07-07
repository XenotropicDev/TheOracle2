using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace TheOracle2.Data.Tests
{
    [TestClass()]
    public class OracleCategoryTests
    {
        [TestMethod()]
        [DataRow(typeof(List<OracleRoot>), "*oracle*.json")]
        [DataRow(typeof(List<AssetRoot>), "*asset*.json")]
        [DataRow(typeof(List<MoveRoot>), "*moves*.json")]
        public void LoadAndGenerateTest(Type T, string searchOption)
        {
            var baseDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Data"));
            var files = baseDir.GetFiles(searchOption);

            Assert.IsTrue(files.Length >= 1);

            foreach (var file in files)
            {
                string text = file.OpenText().ReadToEnd();

                var jsonSettings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };

                var root = JsonConvert.DeserializeObject(text, T, jsonSettings);

                Assert.IsNotNull(root);

                switch (root)
                {
                    case List<AssetRoot> a:
                        Console.WriteLine($"there are {a.Sum(i => i.Assets.Count)} assets in {file.Name}");
                        break;
                    case List<MoveRoot> m:
                        Console.WriteLine($"there are {m.Sum(i => i.Moves.Count)} moves in {file.Name}");
                        break;
                    case List<OracleRoot> o:
                        Console.WriteLine($"there are {o.Sum(i => i.Oracles.Count)} in {file.Name}");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
