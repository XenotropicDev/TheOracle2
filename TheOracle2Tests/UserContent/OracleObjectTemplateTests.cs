using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2.UserContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2.ActionRoller;
using Discord;
using Newtonsoft.Json;

namespace TheOracle2.UserContent.Tests
{
    [TestClass()]
    public class OracleObjectTemplateTests
    {
        [TestMethod()]
        public async Task BuildTest()
        {
            var services = TestServices.GetServices();
            var context = services.GetRequiredService<EFContext>();
            //context.RecreateDB();
            //var oracles = context.Oracles.ToList();

            var npc = new OracleObjectTemplate();
            npc.EntityName = "NPC";
            npc.Title = "oracle:1 oracle:3"; //name
            npc.Author = "NPC";
            npc.Fields.Add(new GameObjectFieldInfo { Title = "First Look", Value = "oracle:4" });
            npc.Fields.Add(new GameObjectFieldInfo { Title = "Disposition", Value = "oracle:5" });
            npc.Fields.Add(new GameObjectFieldInfo { Title = "Role", Value = "oracle:6" });

            npc.FollowupOracles.Add(new FollowUpOracle
            {
                Label = "Callsign",
                Value = "oracle:2",
                Description = "Spacers are often known only by their callsigns"
            });

            npc.FollowupOracles.Add(new FollowUpOracle
            {
                Label = "Goal",
                Value = "oracle:7",
            });

            npc.FollowupOracles.Add(new FollowUpOracle
            {
                Label = "Revealed Aspect",
                Value = "oracle:8",
            });

            var json = JsonConvert.SerializeObject(npc, Formatting.Indented);
            Console.WriteLine(json);

            var factory = ActivatorUtilities.CreateInstance<TableRollerFactory>(services);
            var result = npc.Build(factory);

            Assert.IsFalse(string.IsNullOrWhiteSpace(result.GetEmbeds().FirstOrDefault()?.Title));
            Assert.AreNotEqual("oracle:1", result.GetEmbeds().FirstOrDefault()?.Title);
            Assert.AreEqual(1, result.GetComponents()?.Components?.Count);
        }
    }
}
