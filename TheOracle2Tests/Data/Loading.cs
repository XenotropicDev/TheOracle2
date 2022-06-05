using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TheOracle2.DataClasses;

namespace TheOracle2.UserContent.Tests
{

    public class Demo
    {
        public int MyProperty { get; set; }

        public void DemoFunction() { }
    }


    [TestClass()]
    public class DataLoading
    {
        public DataLoading()
        {

        }

        [TestMethod()]
        [DataRow(typeof(List<AssetRoot>), "asset*.json")]
        //[DataRow(typeof(EncountersRoot), "encounter*.json")]
        //[DataRow(typeof(List<GlossaryRoot>), "glossary*.json")]
        //[DataRow(typeof(MovesInfo), "move*.json")]
        //[DataRow(typeof(List<OracleInfo>), "oracle*.json")]
        //[DataRow(typeof(TruthRoot), "*truth*.json")]
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
                    case List<Asset> a:
                        Console.WriteLine($"there are {a.Count} assets in {file.Name}");
                        break;
                    case List<AssetRoot> ar:
                        Console.WriteLine($"there are {ar.Count} root assets and {ar.SelectMany(r => r.Assets).Count()} assets in {file.Name}");
                        break;
                    case MoveRoot m:
                        Console.WriteLine($"there are {m.Moves.Count} moves in {file.Name}");
                        break;
                    case List<OracleInfo> o:
                        Console.WriteLine($"there are {o.Sum(i => i.Oracles.Count)} in {file.Name}");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
