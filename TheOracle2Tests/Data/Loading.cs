using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration.CSharp;
using OracleData;
using TheOracle2.DataClasses;

namespace TheOracle2.UserContent.Tests
{
    [TestClass()]
    public class DataLoading
    {
        [TestMethod()]
        [DataRow(typeof(AssetRoot), "asset*.json")]
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
            }

            var schema = NJsonSchema.JsonSchema.FromType(T);
            File.WriteAllText(nameof(T) + ".schema.json", schema.ToJson());
        }
    }
}