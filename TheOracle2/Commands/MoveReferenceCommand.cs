using Discord.WebSocket;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2;

public class MoveReferenceCommand : ISlashCommand
{
    public MoveReferenceCommand(UserContent.EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public EFContext DbContext { get; }
    public SocketSlashCommand Context { get; set; }

    [OracleSlashCommand("reference")]
    public async Task GetReferenceMessage()
    {
        int Id = Convert.ToInt32(Context.Data.Options.FirstOrDefault().Options.FirstOrDefault().Value);

        var ephemeral = (Context.Data.Options.FirstOrDefault().Options.FirstOrDefault(o => o.Name == "ephemeral")?.Value as bool?) == true;
        var keepMsg = (Context.Data.Options.FirstOrDefault().Options.FirstOrDefault(o => o.Name == "keep-message")?.Value as bool?) == true;
        var move = DbContext.Moves.Find(Id);

        var builder = new EmbedBuilder()
        .WithAuthor(move.Category)
        .WithTitle(move.Name)
        .WithDescription(move.Text);

        //todo add ephemeral option
        await Context.RespondAsync(embed: builder.Build(), ephemeral: ephemeral).ConfigureAwait(false);

        if (!keepMsg && !ephemeral)
        {
            await Task.Delay(TimeSpan.FromMinutes(1)); //todo: change this to something like 15?
            await Context.DeleteOriginalResponseAsync().ConfigureAwait(false);
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