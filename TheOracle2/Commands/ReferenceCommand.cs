using Discord.WebSocket;
using TheOracle2.UserContent;

namespace TheOracle2;

public class ReferenceCommand : ISlashCommand
{
    public ReferenceCommand(UserContent.EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public SocketSlashCommand Context { get; set; }
    public EFContext DbContext { get; }

    [OracleSlashCommand("reference")]
    public async Task GetReferenceMessage()
    {
        int Id = Convert.ToInt32(Context.Data.Options.FirstOrDefault().Options.FirstOrDefault().Value);

        var move = DbContext.Moves.Find(Id);

        EmbedBuilder builder = new EmbedBuilder();
        builder.WithAuthor(move.Category);
        builder.WithTitle(move.Name);
        builder.Description = move.Text;

        await Context.RespondAsync(embed: builder.Build()).ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromMinutes(15));

        var msg = await Context.GetOriginalResponseAsync();
        await msg.DeleteAsync().ConfigureAwait(false);
    }

    //Todo: Add emphermal option
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

                var option = new SlashCommandOptionBuilder()
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
                option.AddOption(subChoicesOption);

                command.AddOption(option);
            }
        }

        return new List<SlashCommandBuilder>() { command };
    }
}
