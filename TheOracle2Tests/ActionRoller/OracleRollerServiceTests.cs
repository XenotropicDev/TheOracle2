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