using Discord.Interactions;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;
namespace TheOracle2;
// same as ProgressTrackCommandGroup, but as a single command with progress type set via a parameter. only one should be enabled at a time.
// [DontAutoRegister]
public class ProgressTrackCommand : InteractionModuleBase
{
    public EFContext DbContext { get; set; }
    public ProgressTrackCommand(EFContext dbContext)
    {
        DbContext = dbContext;
    }
    [SlashCommand("progress-track", "Create a progress track. For simple progress rolls, use /roll. For scene challenges, use /clock.")]
    public async Task BuildProgressTrack(
    [Summary(description: "A typed progress track includes shortcuts for referencing related moves; generic tracks omit them.")]
    ProgressTrackType trackType,
    [Summary(description: "The track's objective.")]
    string title,
    [Summary(description: "The challenge rank for the progress track.")]
    ChallengeRank rank,
    [Summary(description: "An optional description.")]
    string description="",
    [Summary(description: "A score to pre-set the track, if desired.")]
    [MinValue(0)][MaxValue(10)]
    int score = 0
    )
    {
        switch (trackType)
        {
            case ProgressTrackType.Vow:
                await BuildVowTrack(title, rank, description, score);
                break;
            case ProgressTrackType.Expedition:
                await BuildExpeditionTrack(title, rank, description, score);
                break;
            case ProgressTrackType.Combat:
                await BuildCombatTrack(title, rank, description, score);
                break;
            case ProgressTrackType.Generic:
                await BuildGenericTrack(title, rank, description, score);
                break;
        }
    }
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
    public async Task BuildGenericTrack(
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
public enum ProgressTrackType
{
    Vow,
    Expedition,
    Combat,
    // Connection,
    Generic
}
