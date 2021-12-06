using Discord.Interactions;
using Discord.Interactions.Builders;
using Discord.WebSocket;
using TheOracle2.DataClasses;
using TheOracle2.UserContent;

namespace TheOracle2;

public class MoveReferenceCommand : InteractionModuleBase
{
    public MoveReferenceCommand(UserContent.EFContext dbContext)
    {
        DbContext = dbContext;
    }

    public EFContext DbContext { get; }

    [SlashCommand("reference", "Posts a reference message for the provided move")]
    public async Task GetReferenceMessage(Move move)
    {
        var builder = new EmbedBuilder()
        .WithAuthor(move.Category)
        .WithTitle(move.Name)
        .WithDescription(move.Text);

        //todo add ephemeral option
        await RespondAsync(embed: builder.Build()).ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromMinutes(1)); //todo: change this to something like 15?
        await DeleteOriginalResponseAsync().ConfigureAwait(false);
    }
}

internal sealed class MoveReferenceConverter : TypeConverter<Move>
{
    public MoveReferenceConverter(IServiceProvider services)
    {
        Services = services;
    }

    public IServiceProvider Services { get; }

    public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.SubCommandGroup;

    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
    {
        return Task.FromResult(TypeConverterResult.FromSuccess(new Move {Text = "Some longer description", Name = "Test", Category = "Category" }));
    }

    public override void Write(ApplicationCommandOptionProperties properties, IParameterInfo parameterInfo)
    {
        //var cmdBuilder = new Discord.SlashCommandBuilder();
        
        //var parmBuilder = new SlashCommandParameterBuilder();

        var DbContext = Services.GetRequiredService<EFContext>();
        properties.Options = new List<ApplicationCommandOptionProperties>();

        foreach (var category in DbContext.Moves.Select(a => a.Category).Distinct())
        {
            var chunkedList = DbContext.Moves.ToList()
                .Where(a => a.Category == category && a.Id != 0)
                .OrderBy(a => a.Name)
                .Chunk(SlashCommandOptionBuilder.MaxChoiceCount);

            foreach (var moveGroup in chunkedList)
            {
                string catName = category.Replace(" ", "-");
                if (chunkedList.Count() > 1)
                {
                    catName += $"-{moveGroup.First().Name[..1]}-{moveGroup.Last().Name[..1]}";
                }

                var subCommand = new ApplicationCommandOptionProperties
                {
                    Name = catName.ToLower(),
                    Description = category,
                    IsRequired = false,
                    Type = ApplicationCommandOptionType.SubCommand,
                    Options = new List<ApplicationCommandOptionProperties>(),
                    //Choices = new List<ApplicationCommandOptionChoiceProperties>()
                };

                var catChoices = new ApplicationCommandOptionProperties
                {
                    Name = "move-name",
                    Description = "The name of the move to be posted",
                    IsRequired = true,
                    Type = ApplicationCommandOptionType.Integer,
                    Choices = new List<ApplicationCommandOptionChoiceProperties>()
                };

                foreach (var move in moveGroup)
                {
                    //subCommand.Choices.Add(new ApplicationCommandOptionChoiceProperties { Name = move.Name, Value = move.Id });
                    catChoices.Choices.Add(new ApplicationCommandOptionChoiceProperties { Name = move.Name, Value = move.Id });
                }
                //subCommand.Options.Add(catChoices);

                properties.Options.Add(subCommand);
            }
        }

        properties.IsRequired = false;
        properties.Description = "Reference Moves";

        Console.WriteLine("Command passing back to module");
    }
}