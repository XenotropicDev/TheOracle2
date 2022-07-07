using Discord.Interactions;
using Server.Data;
using Server.DiscordServer;
using Server.GameInterfaces;
using Server.Interactions.Helpers;
using TheOracle2.GameObjects;

namespace TheOracle2;

[Group("progress-track", "Create a progress track. For simple progress rolls, use /roll. For scene challenges, use /clock.")]
public class ProgressTrackCommand : InteractionModuleBase
{
    private readonly IEmoteRepository emotes;
    private readonly IMoveRepository moves;

    public ApplicationContext DbContext { get; set; }
    public Random Random { get; }

    public ProgressTrackCommand(ApplicationContext dbContext, Random random, IEmoteRepository emotes, IMoveRepository moves)
    {
        DbContext = dbContext;
        Random = random;
        this.emotes = emotes;
        this.moves = moves;
    }

    //[SlashCommand("vow", "Create a vow progress track for the Swear an Iron Vow move.")]
    //public async Task BuildVowTrack(
    //[Summary(description: "The vow's objective.")]
    //string title,
    //[Summary(description: "The challenge rank of the progress track.")]
    //ChallengeRank rank,
    //[Summary(description: "An optional description.")]
    //string description="",
    //[Summary(description: "A score to pre-set the track, if desired.")]
    //[MinValue(0)][MaxValue(10)]
    //int score = 0
    //)
    //{
    //    VowTrack track = new(dbContext: DbContext, rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
    //    await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
    //}

    //[SlashCommand("expedition", "Create an expedition progress track for the Undertake an Expedition move.")]
    //public async Task BuildExpeditionTrack(
    //[Summary(description: "The expedition's name.")]
    //string title,
    //[Summary(description: "The challenge rank of the progress track.")]
    //ChallengeRank rank,
    //[Summary(description: "An optional description.")]
    //string description="",
    //[Summary(description: "A score to pre-set the track, if desired.")]
    //[MinValue(0)][MaxValue(10)]
    //int score = 0
    //  )
    //{
    //    ExpeditionTrack track = new(dbContext: DbContext, rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
    //    await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
    //}

    //[SlashCommand("combat", "Create a combat progress track when you Enter the Fray.")]
    //public async Task BuildCombatTrack(
    //[Summary(description: "The combat objective.")]
    //string title,
    //[Summary(description: "The challenge rank of the progress track.")]
    //ChallengeRank rank,
    //[Summary(description: "An optional description.")]
    //string description="",
    //[Summary(description: "A score to pre-set the track, if desired.")]
    //[MinValue(0)][MaxValue(10)]
    //int score = 0
    //)
    //{
    //    CombatTrack track = new(dbContext: DbContext, rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
    //    await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
    //}

    [SlashCommand("generic", "Create a generic progress track.")]
    public async Task BuildGenericTrack(
    [Summary(description: "A title for the progress track.")]
    string title,
    [Summary(description: "The challenge rank of the progress track.")]
    ChallengeRank rank,
    [Summary(description: "An optional description.")]
    string description="",
    [Summary(description: "A score to pre-set the track, if desired.")][MinValue(0)][MaxValue(10)]
    int score = 0)
    {
        ProgressTrack track = new(Random, rank, emotes, moves, title, description, score);        
        await RespondAsync(embeds: track.AsEmbedArray(), components: track.GetComponents()?.Build());
    }
}

public enum ProgressTrackType
{
    Vow,
    Expedition,
    Combat,
    Generic
}
