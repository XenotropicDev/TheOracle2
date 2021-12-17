using Discord.WebSocket;
using TheOracle2.UserContent;

namespace TheOracle2;

public class MoveReferenceCommand : ISlashCommand
{
    public MoveReferenceCommand(UserContent.EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public EFContext DbContext { get; }
    public SocketSlashCommand SlashCommandContext { get; set; }

    [OracleSlashCommand("reference")]
    public async Task GetReferenceMessage()
    {
        int Id = Convert.ToInt32(SlashCommandContext.Data.Options.FirstOrDefault().Options.FirstOrDefault().Value);

        var ephemeral = (SlashCommandContext.Data.Options.FirstOrDefault().Options.FirstOrDefault(o => o.Name == "ephemeral")?.Value as bool?) == true;
        var keepMsg = (SlashCommandContext.Data.Options.FirstOrDefault().Options.FirstOrDefault(o => o.Name == "keep-message")?.Value as bool?) == true;
        var move = DbContext.Moves.Find(Id);

        var builder = new EmbedBuilder()
        .WithAuthor(move.Category)
        .WithTitle(move.Name)
        .WithDescription(move.Text);

        //todo add ephemeral option
        await SlashCommandContext.RespondAsync(embed: builder.Build(), ephemeral: ephemeral).ConfigureAwait(false);

        if (!keepMsg && !ephemeral)
        {
            await Task.Delay(TimeSpan.FromMinutes(15)).ConfigureAwait(false);
            await SlashCommandContext.DeleteOriginalResponseAsync().ConfigureAwait(false);
        }
    }

    //Todo: Add ephemeral option
    public IList<SlashCommandBuilder> GetCommandBuilders()
    {
        var command = new SlashCommandBuilder()
            .WithName("reference")
            .WithDescription("Posts the game text for a move");

        foreach (var category in DbContext.Moves.Select(a => a.Category).Distinct())
        {
            var chunkedList = DbContext.Moves.ToList()
                .Where(a => a.Category == category && a.Id != 0)
                .OrderBy(a => a.Name)
                .Chunk(SlashCommandOptionBuilder.MaxChoiceCount);

            foreach (var moveGroup in chunkedList)
            {
                string name = category.Replace(" ", "-");
                if (chunkedList.Count() > 1)
                {
                    name += $"-{moveGroup.First().Name.Substring(0, 1)}-{moveGroup.Last().Name.Substring(0, 1)}";
                }

                var subCommand = new SlashCommandOptionBuilder()
                    .WithName(name.ToLower())
                    .WithDescription($"{category} moves")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    ;

                var subChoicesOption = new SlashCommandOptionBuilder()
                    .WithName("move-name")
                    .WithDescription("The name of the move to be posted")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Integer);

                foreach (var move in moveGroup)
                {
                    subChoicesOption.AddChoice(move.Name, move.Id);
                }
                subCommand.AddOption(subChoicesOption);
                subCommand.AddOption("ephemeral", ApplicationCommandOptionType.Boolean, "Display the message only to you");
                subCommand.AddOption("keep-message", ApplicationCommandOptionType.Boolean, "Prevents the bot from automatically clean up the message");

                command.AddOption(subCommand);
            }
        }

        return new List<SlashCommandBuilder>() { command };
    }
}

public class DelayCommand
{
    private SocketSlashCommand SlashCommand;

    public DelayCommand(SocketSlashCommand slashCommand)
    {
        this.SlashCommand = slashCommand;
    }

    public async Task DelayTest()
    {
        var msg = await SlashCommand.RespondAsync("Message to be deleted").ConfigureAwait(false);
        await Task.Delay(5000).ConfigureAwait(false);
        await msg.DeleteAsync().ConfigureAwait(false);
    }
}