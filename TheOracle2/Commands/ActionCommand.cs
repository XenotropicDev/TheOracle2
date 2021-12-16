using Discord.Interactions;

namespace TheOracle2;

public class ActionCommand : InteractionModuleBase
{
    [SlashCommand("action", "Performs an Ironsworn action roll")]
    public async Task Action(
        [Summary(description: "The total bonus to add to the action die result")] int modifiers,
        [Summary(description: "Any role playing text you'd like to include in the post")] string fluff = "")
    {
        var roll = new ActionRoll(modifiers, message: fluff);
        await ReplyAsync(embed: roll.ToEmbed().Build());
    }

    [SlashCommand("progress-roll", "Perform a progress (or similar) roll")]
    public async Task ProgressRoll(
    [Summary(description: "The progress value/score to use")] int score,
    [Summary(description: "Any role playing text you'd like to include in the post")] string fluff = "")
    {
        var roll = new ActionRoll(0, score, fluff);
        await ReplyAsync(embed: roll.ToEmbed().WithAuthor("Progress Roll").Build());
    }
}
