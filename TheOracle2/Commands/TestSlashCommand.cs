﻿using Discord.WebSocket;

namespace TheOracle2;

public class TestSlashCommand : ISlashCommand
{
    public SocketSlashCommand Context { get; set; }

    public IList<SlashCommandBuilder> GetCommandBuilders()
    {
        var pingCommand = new SlashCommandBuilder();
        pingCommand.WithName("ping");
        pingCommand.WithDescription("Test slash command");

        return new List<SlashCommandBuilder>() { pingCommand };
    }

    [OracleSlashCommand("ping")]
    public async Task Ping()
    {
        await Context.RespondAsync($"Pong!", ephemeral: true).ConfigureAwait(false);
    }
}
