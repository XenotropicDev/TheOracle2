using System.Text.RegularExpressions;
using TheOracle2.ActionRoller;

namespace TheOracle2.UserContent;

public interface IGameObjectTemplate
{
    IDiscordEntity Build(TableRollerFactory rollerFactory);
}

/* Things to solve/support:
 * Specific rows
 * Field names from results
 * Custom selects/buttons (from other handeled bot features)
 */

public class OracleObjectTemplate : IGameObjectTemplate
{
    public int Id { get; set; }
    public string EntityName { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Image { get; set; }
    public bool IsEphemeralByDefault { get; set; } = false;
    public virtual List<GameObjectFieldInfo> Fields { get; set; } = new();
    public virtual List<FollowUpOracle> FollowupOracles { get; set; } = new();

    public IDiscordEntity Build(TableRollerFactory rollerFactory)
    {
        var entity = new DiscordEntity();
        var comp = new ComponentBuilder();
        var embed = new EmbedBuilder();

        embed.WithAuthor(RollValueFacade(rollerFactory, Author, LookupMethod.UseFirst).Item1);
        embed.WithTitle(RollValueFacade(rollerFactory, Title, LookupMethod.UseFirst).Item1);

        comp.WithSelectMenu("add-oracle-select", FollowupOracles.Select(o => o.ToBuilder()).ToList());

        foreach (var field in this.Fields)
        {
            var rollValues = RollValueFacade(rollerFactory, field.Value, field.LookupMethod);

            embed.Fields.Add(new EmbedFieldBuilder().WithValue(rollValues.Item1).WithName(field.Title).WithIsInline(field.IsInline));

            Image ??= rollValues.Item2.TableResult.Image;

            var builders = new DiscordOracleBuilder(rollValues.Item2).Build();
            if (builders.ComponentBuilder != null)
            {
                comp.MergeComponents(builders.ComponentBuilder);
            }
        }

        entity.WithEmbed(embed.Build());
        entity.WithComponent(comp.Build());
        entity.IsEphemeral = IsEphemeralByDefault;
        return entity;
    }

    private Tuple<string, OracleRollerResult> RollValueFacade(TableRollerFactory rollerFactory, string value, LookupMethod method)
    {
        var rollerRegex = new Regex(@"(oracle|tables?|subcat):\d+", RegexOptions.IgnoreCase);
        OracleRollerResult returnedRoller = null; //Todo: maybe support returning all the rollers?

        foreach (Match match in rollerRegex.Matches(value).Reverse())
        {
            var roller = rollerFactory.GetRoller(match.Value)?.Build();

            if (roller == null) continue;

            var desc = string.Empty;
            switch (method)
            {
                case LookupMethod.UseFirst:
                    returnedRoller = roller;
                    desc = roller.TableResult.Description;
                    break;

                case LookupMethod.UseLast:
                    returnedRoller ??= roller;
                    var last = roller.GetLastResults();
                    desc = string.Join(" / ", last.Select(rr => rr.TableResult.Description));
                    break;

                case LookupMethod.UseAll:
                    returnedRoller = roller;
                    var allDescriptions = roller.ChildResults?.Select(rr => rr.TableResult.Description).ToList();
                    if (string.IsNullOrWhiteSpace(roller.TableResult?.Description)) allDescriptions.Insert(0, roller.TableResult.Description);
                    desc = string.Join(" / ", allDescriptions);
                    break;

                default:
                    break;
            }
            value = value.Remove(match.Index, match.Length).Insert(match.Index, desc);
        }

        return new Tuple<string, OracleRollerResult>(value, returnedRoller);
    }
}

public class FollowUpOracle
{
    public int Id { get; set; }
    public int TemplateId { get; set; }
    public virtual OracleObjectTemplate Template { get; set; }

    public string Label { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public string Emote { get; set; }
    public bool? IsDefault { get; set; }

    public SelectMenuOptionBuilder ToBuilder()
    {
        return new SelectMenuOptionBuilder().WithDescription(Description).WithValue(Value).WithLabel(Label).WithEmote(new Emoji(Emote)).WithDefault(IsDefault == true);
    }
}

public class GameObjectFieldInfo
{
    public int Id { get; set; }
    public int TemplateId { get; set; }
    public virtual OracleObjectTemplate Template { get; set; }
    public string Value { get; set; }
    public LookupMethod LookupMethod { get; set; } = LookupMethod.UseFirst;
    public bool IsInline { get; set; } = true;
    public string Title { get; set; }
}

public enum LookupMethod
{
    UseFirst,
    UseLast,
    UseAll,
}
