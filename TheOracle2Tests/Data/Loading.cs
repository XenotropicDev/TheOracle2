using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
//using NJsonSchema.CodeGeneration.CSharp;
using OracleData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheOracle2.DataClasses;

namespace TheOracle2.UserContent.Tests
{
    [TestClass()]
    public class DataLoading
    {
        [TestMethod()]
        [DataRow(typeof(List<Asset>), "asset*.json")]
        [DataRow(typeof(EncountersRoot), "encounter*.json")]
        [DataRow(typeof(List<GlossaryRoot>), "glossary*.json")]
        [DataRow(typeof(MovesInfo), "move*.json")]
        [DataRow(typeof(List<OracleInfo>), "oracle*.json")]
        [DataRow(typeof(TruthRoot), "*truth*.json")]
        public void LoadAndGenerateTest(Type T, string searchOption)
        {
            var baseDir = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Data");
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
                    case AssetRoot a:
                        Console.WriteLine($"there are {a.Assets.Count} assets in {file.Name}");
                        break;
                    case MovesInfo m:
                        Console.WriteLine($"there are {m.Moves.Count} moves in {file.Name}");
                        break;
                    case List<OracleInfo> o:
                        Console.WriteLine($"there are {o.Sum(i => i.Oracles.Count)} in {file.Name}");
                        break;
                    default:
                        break;
                }
            }

            //var schema = NJsonSchema.JsonSchema.FromType(T);
            //File.WriteAllText(nameof(T) + ".schema.json", schema.ToJson());
        }
    }
}