using System.Text.RegularExpressions;
using Discord.Interactions;
using Discord.WebSocket;
using Server.DiceRoller;
using Server.Interactions.Helpers;

namespace TheOracle2;

[Group("roll", "Make an action roll (p. 28) or progress roll (p. 39). For oracle tables, use '/oracle'")]
public class RollCommandGroup : InteractionModuleBase
{
    public RollCommandGroup(Random random)
    {
        Random = random;
    }

    public Random Random { get; }
    
    [SlashCommand("action", "Make an action roll (p. 28) by setting a stat value.")]
    public async Task RollAction(
        [Summary(description: "The stat value to use for the roll")] int stat,
        [Summary(description: "Any adds to the roll")][MinValue(0)] int adds,
        [Summary(description: "The player character's momentum.")][MinValue(-6)][MaxValue(10)] int momentum,
        [Summary(description: "Any notes, fiction, or other text you'd like to include with the roll")] string description = "",
        [Summary(description: "A preset value for the Action Die (d6) to use instead of rolling.")][MinValue(1)][MaxValue(6)] int? actionDie = null,
        [Summary(description: "A preset value for the first Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie1 = null,
        [Summary(description: "A preset value for the second Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie2 = null)
    {
        var roll = new ActionRollRandom(Random, stat, adds, momentum, description, actionDie, challengeDie1, challengeDie2);
        await RespondAsync(embeds: roll.AsEmbedArray()).ConfigureAwait(false);
    }
    [SlashCommand("progress", "Roll with a set progress score (p. 39). For an interactive progress tracker, use /progress-track.")]
    public async Task RollProgress(
        [Summary(description: "The progress score.")] int progressScore,
        [Summary(description: "A preset value for the first Challenge Die to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie1 = null,
        [Summary(description: "A preset value for the second Challenge Die to use instead of rolling")][MinValue(1)][MaxValue(10)] int? challengeDie2 = null,
        [Summary(description: "Notes, fiction, or other text to include with the roll.")] string description = "")
    {
        var roll = new ProgressRollRandom(Random, progressScore, description, challengeDie1, challengeDie2);
        await RespondAsync(embeds: roll.AsEmbedArray()).ConfigureAwait(false);
    }
}
