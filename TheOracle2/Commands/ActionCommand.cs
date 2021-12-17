using Discord.Interactions;
using Discord.WebSocket;

namespace TheOracle2;

public class ActionCommand : InteractionModuleBase
{
    [SlashCommand("action", "Performs an Ironsworn action roll")]
    public async Task Action(
        [Summary(description: "The stat value to use for the roll")] int stat,
        [Summary(description: "Any additional bonuses to the roll")] int adds = 0,
        [Summary(description: "Any role playing text you'd like to include in the post")] string fiction = "")
    {
        var roll = new ActionRoll(stat + adds, message: fiction);
        await RespondAsync(embed: roll.ToEmbed().Build()).ConfigureAwait(false);
    }

    [SlashCommand("progress-roll", "Perform a progress (or similar) roll")]
    public async Task ProgressRoll(
    [Summary(description: "The progress value/score to use")] int score,
    [Summary(description: "Any role playing text you'd like to include in the post")] string fiction = "")
    {
        var roll = new ActionRoll(0, score, fiction);
        await RespondAsync(embed: roll.ToEmbed().WithAuthor("Progress Roll").Build()).ConfigureAwait(false);
    }
}