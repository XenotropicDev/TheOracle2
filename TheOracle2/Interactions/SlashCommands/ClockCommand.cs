using Discord.Interactions;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;
namespace TheOracle2;
// same as ClockCommandGroup, but as a single command with clock type set via a parameter. only one should be enabled at a time.
// [DontAutoRegister]
public class ClockCommand : InteractionModuleBase
{
    public EFContext DbContext { get; set; }
    public ClockCommand(EFContext dbContext)
    {
        DbContext = dbContext;
    }
    [SlashCommand("clock", "Set a campaign clock, tension clock, or scene challenge (p. 230).")]
    public async Task BuildClock(
        [Summary(description: "The type of clock: campaign clock (p. 231), tension clock (p. 234), or scene challenge (p. 235).")]
        [Choice("Campaign clock","campaign-clock"),
        Choice("Tension clock", "tension-clock"),
        Choice("Scene challenge", "scene-challenge")]
        string clockType,
        [Summary(description: "A title that makes it clear what project is complete or event triggered when the clock is filled.")]
        string title,
        [Summary(description: "The number of clock segments.")]
        ClockSize segments,
        [Summary(description: "An optional description.")]
        string description=""
    )
    {
        switch (clockType)
        {
            case "campaign-clock":
                await BuildCampaignClock(title, segments, description);
                break;
            case "tension-clock":
                await BuildTensionClock(title, segments, description);
                break;
            case "scene-challenge":
                await BuildSceneChallenge(title, segments, description);
                break;
        }
    }
    // [SlashCommand("campaign", "Set a campaign clock to resolve objectives and actions in the background of your campaign (p. 231).")]
    public async Task BuildCampaignClock(
      [Summary(description: "A title that makes it clear what project is complete or event triggered when the clock is filled.")]
    string title,
      [Summary(description: "The number of clock segments.")]
    ClockSize segments,
      [Summary(description: "An optional description.")]
    string description=""
    )
    {
        CampaignClock campaignClock = new(segments, 0, title, description);
        await RespondAsync(
          embed: campaignClock.ToEmbed().Build(),
          components: campaignClock.MakeComponents().Build()
          );
    }
    // [SlashCommand("tension", "Set a tension clock: a smaller-scope clock to fill as you suffer setbacks or fail to act (p. 234).")]
    public async Task BuildTensionClock(
      [Summary(description: "A title for the tension clock.")]
    string title,
      [Summary(description: "The number of clock segments. Imminent danger or deadline: 4-6. Longer term threat: 8-10.")]
    ClockSize segments,
      [Summary(description: "An optional description.")]
    string description=""
    )
    {
        TensionClock tensionClock = new(segments, 0, title, description);
        await RespondAsync(
          embed: tensionClock.ToEmbed().Build(),
          components: tensionClock.MakeComponents().Build());
    }
    // [SlashCommand("scene-challenge", "Create a scene challenge for extended non-combat scenes against threats or other characters (p. 235).")]
    public async Task BuildSceneChallenge(
      [Summary(description: "The scene challenge's objective.")]
    string title,
      [Summary(description: "The number of clock segments. Default = 6, severe disadvantage = 4, strong advantage = 8.")]
    ClockSize segments=ClockSize.Six,
      [Summary(description: "An optional description.")]
    string description = "",
      [Summary(description: "A score to pre-set the track, if desired.")] [MinValue(0)][MaxValue(10)]
    int score = 0)
    {
        SceneChallenge sceneChallenge = new(DbContext, segments, 0, ticks: score * ITrack.BoxSize, title: title, description: description);
        EmbedBuilder embed = sceneChallenge.ToEmbed();
        ComponentBuilder components = sceneChallenge.MakeComponents();
        await RespondAsync(
          embed: embed.Build(),
          components: components.Build()
          );
    }
}
