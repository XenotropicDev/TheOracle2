using TheOracle2.UserContent;

namespace TheOracle2.GameObjects;

/// <summary>
/// Interface for all ranked progress tracks.
/// </summary>
public interface IProgressTrack : ITrack, ILogWidget, IRanked
{
    /// <summary>
    /// Adds fields to represent a ranked progress track to an EmbedBuilder.
    /// </summary>
    public static EmbedBuilder ProgressTemplate(IProgressTrack progressTrack, EmbedBuilder embed)
    {
        embed.WithFields(
          RankField(progressTrack.Rank)
            .WithIsInline(true),
          TrackField(progressTrack.Ticks)
            .WithIsInline(true)
          );
        return embed;
    }

    /// <summary>
    /// Marks ticks of progress on the track, and returns an alert embed (with a custom title) to notify the player of the change.
    /// </summary>
    /// <param name="addTicks">The number of ticks to add.</param>
    /// <param name="alertTitle">A title for the alert.</param>
    public EmbedBuilder Mark(int addTicks, string alertTitle);

    /// <summary>
    /// Marks ticks of progress on the track, and returns an alert embed (titled with the default progress marking move of the track) to notify the player of the change.
    /// </summary>
    public EmbedBuilder Mark(int addTicks);

    /// <summary>
    /// Marks one unit of progress on the track, and returns an alert embed (titled with the default progress marking move of the track) to notify the player of the change.
    /// </summary>
    public EmbedBuilder Mark();

    public static ProgressTrack FromEmbed(EFContext dbContext, Embed embed, bool alerts = false)
    {
        switch (embed.Author.ToString())
        {
            case "Progress Track":
                return new GenericTrack(dbContext, embed, alerts);

            case "Scene Challenge":
                return new SceneChallenge(dbContext, embed, alerts);

            case "Vow Progress Track":
                return new VowTrack(dbContext, embed, alerts);

            case "Combat Objective Progress Track":
                return new CombatTrack(dbContext, embed, alerts);

            case "Expedition Progress Track":
                return new ExpeditionTrack(dbContext, embed, alerts);

            default:
                break;
        }
        throw new Exception("Unable to parse embed into progress track.");
    }

    /// <summary>
    /// Builds embed without parsing the progress bar string. Might be faster than an embed alone if you can parse the ticks from a CustomId.
    /// </summary>
    public static ProgressTrack FromEmbed(EFContext dbContext, Embed embed, int ticks, bool alerts = false)
    {
        switch (embed.Author.ToString())
        {
            case "Progress Track":
                return new GenericTrack(dbContext, embed, ticks);

            case "Scene Challenge":
                return new SceneChallenge(dbContext, embed, ticks);

            case "Vow Progress Track":
                return new VowTrack(dbContext, embed, ticks);

            case "Combat Objective Progress Track":
                return new CombatTrack(dbContext, embed, ticks);

            case "Expedition Progress Track":
                return new ExpeditionTrack(dbContext, embed, ticks);

            default:
                break;
        }
        throw new Exception("Unable to parse embed into progress track.");
    }

    public static SelectMenuOptionBuilder ResolveOption(ProgressTrack track)
    {
        string moveName = track.ResolveMoveName;
        string description = "Roll progress";
        SelectMenuOptionBuilder option = new SelectMenuOptionBuilder()
          .WithEmote(Emoji["roll"]);
        if (string.IsNullOrEmpty(moveName))
        {
            option.WithLabel(description)
              .WithValue($"progress-roll");
        }
        else
        {
            option.WithLabel(moveName)
              .WithDescription(description)
              .WithValue($"progress-roll");
        }
        return option;
    }

    public static SelectMenuOptionBuilder MarkOption(ProgressTrack track, int addTicks)
    {
        string alertLabel = track.MarkAlertTitle;
        string description = $"Mark {TickString(addTicks)} of progress";
        SelectMenuOptionBuilder option = new SelectMenuOptionBuilder().WithEmote(TickEmoji[Math.Min(BoxSize, addTicks)]).WithValue($"progress-mark:{addTicks}");
        if (string.IsNullOrEmpty(alertLabel)) { option.WithLabel(description); }
        if (!string.IsNullOrEmpty(alertLabel))
        {
            option
              .WithLabel(alertLabel)
              .WithDescription(description);
        }
        return option;
    }

    public static SelectMenuOptionBuilder ClearOption(int subtractTicks)
    {
        return new SelectMenuOptionBuilder()
          .WithLabel($"Clear {TickString(subtractTicks)} of progress")
          .WithEmote(TickEmoji[0])
          .WithValue($"progress-clear:{subtractTicks}")
          ;
    }

    public static SelectMenuOptionBuilder RecommitOption(ProgressTrack track)
    {
        SelectMenuOptionBuilder option = new SelectMenuOptionBuilder()
          .WithLabel("Recommit")
          .WithValue($"progress-recommit")
          .WithEmote(Emoji["recommit"])
        ;
        option.WithDescription($"Recommit after a Miss on {track.ResolveMoveName}.");
        return option;
    }
}
