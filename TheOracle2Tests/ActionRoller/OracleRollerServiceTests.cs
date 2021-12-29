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
        
        var roller = new OracleRollerService(new Random(1), context);

        var oracle = context.Oracles.FirstOrDefault(o => o.Name == "Planetary Class");

        var result = roller.Roll(oracle);

        //var embed = result.GetEmbedBuilder();
        Assert.IsNotNull(result.ChildResults.FirstOrDefault()?.Result.Oracle?.OracleInfo);
        Assert.AreEqual(1, result.ChildResults.Count);
        Assert.AreEqual(4, result.ChildResults.FirstOrDefault().FollowUpTables.Count);
        Assert.IsTrue(result.Result.Description.Length > 0);
    }

    [TestMethod()]
    public async Task LinkedInfoTest()
    {
        var services = TestServices.GetServices();
        var context = services.GetRequiredService<EFContext>();
        //await context.RecreateDB();

        var oracle = context.Oracles.FirstOrDefault(o => o.Name == "Planetary Class");

        Assert.IsNotNull(oracle.OracleInfo);
    }
}
