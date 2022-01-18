﻿using Discord.Interactions;
using TheOracle2.GameObjects;

namespace TheOracle2;

public class OracleAskPaths : InteractionModuleBase {
    private readonly Random random;

    public OracleAskPaths(Random random) {
        this.random = random;
    }

    [SlashCommand("ask", "Ask the oracle based on the predefined likelihoods")]
    public async Task AskTheOracle(
        [Summary(description: "The odds of receiving a 'yes'.")]
        [Choice("Sure thing (10 or less)", 10),
        Choice("Likely (25 or less)", 25),
        Choice("50/50 (50 or less)", 50),
        Choice("Unlikely (75 or less)", 75),
        Choice("Small chance (90 or less)", 90)]
        int odds,
        [Summary(description: "The question to ask the oracle.")]
        string question
    ) {

        /// TODO: once discord display string attributes are available for enums, this can use the AskOption enum directly
        await RespondAsync(embed: new OracleAnswer(random, (AskOption)odds, question).ToEmbed().Build()).ConfigureAwait(false);
    }
}