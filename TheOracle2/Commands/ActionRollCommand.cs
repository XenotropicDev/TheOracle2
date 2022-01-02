﻿using Discord.Interactions;
using Discord.WebSocket;

namespace TheOracle2;

public class ActionRollCommand : InteractionModuleBase {
  public ActionRollCommand(Random random) {
    Random = random;
  }
  public Random Random { get; }
  [SlashCommand("action-roll", "Performs an Ironsworn action roll.")]
  public async Task ActionRoll(
      [Summary(description: "The stat value to use for the roll")][MinValue(0)] int stat,
      [Summary(description: "Any adds to the roll")][MinValue(0)] int adds,
      [Summary(description: "The player character's momentum.")][MinValue(-6)][MaxValue(10)] int momentum,
      [Summary(description: "A preset value for the Action Die (d6) to use instead of rolling.")][MinValue(1)][MaxValue(6)] int? actionDie = null,
      [Summary(description: "A preset value for the first Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie1 = null,
      [Summary(description: "A preset value for the second Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie2 = null,
      [Summary(description: "Any notes, fiction, or other text you'd like to include in the post")] string text = "") {
    var roll = new ActionRoll(stat, adds, momentum, text, actionDie, challengeDie1, challengeDie2);
    await RespondAsync(embed: roll.ToEmbed().Build()).ConfigureAwait(false);
  }
}