using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2.UserContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2Tests;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema.CodeGeneration.CSharp;
using System.IO;
using Newtonsoft.Json;
using OracleData;

namespace TheOracle2.UserContent.Tests
{
    [TestClass()]
    public class DataLoading
    {
        [TestMethod()]
        public async Task GameItemTest()
        {
            //Assert.Inconclusive();

            var baseDir = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Data");
            foreach (var file in baseDir.GetFiles("*Schema.json"))
            {
                string text = file.OpenText().ReadToEnd();
                var schema = await NJsonSchema.JsonSchema.FromJsonAsync(text);
                var gen = new CSharpGenerator(schema);
                var csfile = gen.GenerateFile();

                File.WriteAllText(file.Directory + file.Name + ".cs", csfile);
            }
        }

        [TestMethod()]
        public void LoadAssetsTest()
        {
            var baseDir = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Data");
            var file = baseDir.GetFiles("asset*.json").FirstOrDefault();
            string text = file.OpenText().ReadToEnd();

            var jsonSeetings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };

            var assetRoot = JsonConvert.DeserializeObject<AssetRoot>(text, jsonSeetings);

            Console.WriteLine(assetRoot.Assets.Count); 
        }
    }
}