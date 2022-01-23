using Discord.Interactions;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2;

// same as ProgressTrackCommand, but as a command group. only one should be enabled at a time.

[DontAutoRegister]

[Group("progress-track", "Create a progress track. For simple progress rolls, use /roll. For scene challenges, use /clock.")]
public class ProgressTrackCommandGroup : InteractionModuleBase
{
    public EFContext DbContext { get; set; }

    public ProgressTrackCommandGroup(EFContext dbContext)
    {
        DbContext = dbContext;
    }

    [SlashCommand("vow", "Create a vow progress track for the Swear an Iron Vow move.")]
    public async Task BuildVowTrack(
    [Summary(description: "The vow's objective.")]
    string title,
    [Summary(description: "The challenge rank of the progress track.")]
    ChallengeRank rank,
    [Summary(description: "An optional description.")]
    string description="",
    [Summary(description: "A score to pre-set the track, if desired.")]
    [MinValue(0)][MaxValue(10)]
    int score = 0
    )
    {
        VowTrack track = new(dbContext: DbContext, rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
        await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
    }

    // [SlashCommand("connection", "Create a connection progress track for an NPC.")]

    // TODO: revisit this once there's a good system for NPC embeds

    [SlashCommand("expedition", "Create an expedition progress track for the Undertake an Expedition move.")]
    public async Task BuildExpeditionTrack(
    [Summary(description: "The expedition's name.")]
    string title,
    [Summary(description: "The challenge rank of the progress track.")]
    ChallengeRank rank,
    [Summary(description: "An optional description.")]
    string description="",
    [Summary(description: "A score to pre-set the track, if desired.")]
    [MinValue(0)][MaxValue(10)]
    int score = 0
      )
    {
        ExpeditionTrack track = new(dbContext: DbContext, rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
        await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
    }

    [SlashCommand("combat", "Create a combat progress track when you Enter the Fray.")]
    public async Task BuildCombatTrack(
    [Summary(description: "The combat objective.")]
    string title,
    [Summary(description: "The challenge rank of the progress track.")]
    ChallengeRank rank,
    [Summary(description: "An optional description.")]
    string description="",
    [Summary(description: "A score to pre-set the track, if desired.")]
    [MinValue(0)][MaxValue(10)]
    int score = 0
    )
    {
        CombatTrack track = new(dbContext: DbContext, rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
        await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
    }

    [SlashCommand("generic", "Create a generic progress track")]
    public async Task BuildProgressTrack(
    [Summary(description: "A title for the progress track.")]
    string title,
    [Summary(description: "The challenge rank of the progress track.")]
    ChallengeRank rank,
    [Summary(description: "An optional description.")]
    string description="",
    [Summary(description: "A score to pre-set the track, if desired.")][MinValue(0)][MaxValue(10)]
    int score = 0
    )
    {
        GenericTrack track = new(dbContext: DbContext, rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
        await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
    }
}
