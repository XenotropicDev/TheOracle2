using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2.DiscordHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace TheOracle2.DiscordHelpers.Tests
{
    [TestClass()]
    public class ComponentExtenstionsTests
    {
        [TestMethod()]
        public void RemoveComponentByIdTest()
        {
            var builder = new ComponentBuilder()
                .WithButton("label", "id1", row: 0)
                .WithButton("label", "id2", row: 1)
                .WithButton("label", "id3", row: 1);

            builder.RemoveComponentById("id2");

            Assert.IsFalse(builder.ActionRows.Any(r => r.Components.Any(c => c.CustomId == "id2")));

            builder.RemoveComponentById("id3");
            Assert.AreEqual(1, builder.ActionRows.Count);
        }

        [TestMethod()]
        public void TryAddTest()
        {
            var builder = new ComponentBuilder()
            .WithButton("label", "id1", row: 0)
            .WithButton("label", "id2", row: 1)
            .WithButton("label", "id3", row: 1);

            builder.TryAdd(ButtonBuilder.CreateSuccessButton("label","id4").Build(), 2);
        }
    }
}