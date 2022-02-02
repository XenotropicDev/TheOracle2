using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2.UserContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2.ActionRoller;
using Discord;

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
            npc.Title = "oracle:1"; //name
            npc.Author = "NPC, no oracle roll";
            npc.Fields.Add(new GameObjectFieldInfo { Title = "Callsign", Value = "oracle:2" });
            npc.Fields.Add(new GameObjectFieldInfo { Title = "Full Name", Value = "oracle:1 oracle:3" });

            var oracleOption = new FollowUpOracle();
            oracleOption.Label = "Disposition";
            oracleOption.Value = "oracle:15";
            oracleOption.Description = "extra text";
            npc.FollowupOracles.Add(oracleOption);

            var factory = ActivatorUtilities.CreateInstance<TableRollerFactory>(services);
            var result = npc.Build(factory);

            Assert.IsFalse(string.IsNullOrWhiteSpace(result.GetEmbeds().FirstOrDefault()?.Title));
            Assert.AreNotEqual("oracle:1", result.GetEmbeds().FirstOrDefault()?.Title);
            Assert.AreEqual(1, result.GetComponents()?.Components?.Count);
        }
    }
}
