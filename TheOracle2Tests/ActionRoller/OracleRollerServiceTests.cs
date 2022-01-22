using TheOracle2.ActionRoller;

namespace TheOracle2.Tests;

[TestClass()]
public class OracleRollerServiceTests
{
    [TestMethod()]
    public async Task RollTest()
    {
        var services = TestServices.GetServices();
        var context = services.GetRequiredService<EFContext>();
        //await context.RecreateDB();

        var oracle = context.Oracles.FirstOrDefault(o => o.Name == "Planetary Class");
        
        var result = new OracleRoller(new Random(1), context, oracle)
            .Build();

        //Assert.IsNotNull(result.ChildResults.FirstOrDefault()?.Result.Oracle?.OracleInfo);
        Assert.AreEqual(1, result.ChildResults.Count);
        Assert.AreEqual(4, result.FollowUpTables.Count);
        Assert.IsTrue(result.Result.Description.Length > 0);
    }

    [TestMethod()]
    public async Task RollerFactoryTest()
    {
        var services = TestServices.GetServices();
        var context = services.GetRequiredService<EFContext>();
        var random = services.GetRequiredService<Random>();
        var RollerFactory = new TableRollerFactory(context, random);

        var subCat = context.OracleInfo.SelectMany(o => o.Subcategories.Where(s => s.Name == "Desert World")).ToList();

        foreach (var cat in subCat)
        {
            Console.WriteLine(cat.Name);
        }

        var rollResult = RollerFactory.GetRoller("tables:17").Build();
        var rollResult2 = RollerFactory.GetRoller("subcat:16").Build();
        var rollResult3 = RollerFactory.GetRoller("16").Build();

        Assert.AreEqual("Planetside Peril - Lifeless", rollResult?.Name);
        Assert.AreEqual("Desert World", rollResult2?.Name);
        Assert.AreEqual("Theme", rollResult3?.Name);
    }

    [TestMethod()]
    public async Task RollerEmbedTest()
    {
        var services = TestServices.GetServices();
        var context = services.GetRequiredService<EFContext>();

        var oracle = context.Oracles.FirstOrDefault(o => o.Name == "Planetary Class");
        var roller = new OracleRoller(new Random(1), context, oracle);

        var result = roller.Build();

        var embed = new DiscordOracleBuilder(result).Build().EmbedBuilder;

        foreach (var field in embed.Fields)
        {
            Assert.AreEqual(1, embed.Fields.Count(f => f.Name == field.Name));
        }
    }
}
