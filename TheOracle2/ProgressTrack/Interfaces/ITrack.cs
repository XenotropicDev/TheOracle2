namespace TheOracle2.GameObjects;
using System.Text.RegularExpressions;

/// <summary>
/// Interface inherited by all ranked and unranked tracks.
/// </summary>
public interface ITrack
{
  /// <summary>
  /// The number of ticks per progress box.
  /// </summary>
  public const int BoxSize = 4;
  /// <summary>
  /// The number of progress boxes per progress track.
  /// </summary>
  public const int TrackSize = 10;
  /// <summary>
  /// The number of ticks per progress track.
  /// </summary>
  public const int MaxTicks = BoxSize * TrackSize;

  public int Ticks { get; set; }
  public int Score { get; }
  public ProgressRoll Roll(Random random);

  /// <summary>
  /// Calculates a progress score from a given number of ticks, capped by TrackSize.
  /// </summary>
  public static int GetScore(int ticks)
  {
    int rawScore = ticks / BoxSize;
    return Math.Max(0, Math.Min(rawScore, TrackSize));
  }
  /// <summary>
  /// Special hidden character for mobile formatting small emojis.
  /// </summary>
  public const string MobileEmojiSizer = "\u200C";

  private static string MarkedBoxesPattern => "/:progress([1-4]):/";

  /// <summary>
  /// Counts the ticks marked in an emoji-based progress track. Only boxes with at least one tick are counted; other characters/substrings are ignored.
  /// </summary>
  /// <param name="emojiString">The progress track string. </param>

  public static int ParseTrack(string emojiString)
  {
    var matchStrings = Regex.Matches(emojiString, MarkedBoxesPattern);
    var boxValues = matchStrings.Select(match => int.Parse(match.ToString()));
    return boxValues.Sum();
  }
  private static string PartialBoxesPattern => "/:progress([1-3]):/";
  private static string ProgressFieldPattern => $"Track {Regex.Escape("[")}([0-9]|{TrackSize})/{TrackSize}{Regex.Escape("]")}";
  /// <summary>
  /// Counts the ticks in an EmbedField that represents a progress track. Might be more efficient than parsing from an emoji-based track because it doesn't iterate as much.
  /// </summary>
  public static int ParseTrack(EmbedField embedField)
  {
    // string scoreString = Regex.Match(embedField.Name, ProgressFieldPattern).Value;
    string scoreString = embedField.Name.Split('[')[1].Split('/')[0];

    if (!int.TryParse(scoreString, out int score))
    {
      throw new Exception($"Unable to parse {nameof(score)} from {scoreString}");
    }

    string remainderTicksString = Regex.Match(embedField.Value, PartialBoxesPattern).ToString();

    if (!int.TryParse(remainderTicksString, out int remainderTicks))
    {
      throw new Exception($"Unable to parse {nameof(remainderTicks)} from {remainderTicksString}");
    }

    int ticks = (BoxSize * score) + remainderTicks;
    return ticks;
  }
  /// <summary>
  /// Counts the ticks of the first progress track in an Embed. Might be more efficient than parsing from an emoji-based track because it doesn't iterate as much.
  /// </summary>
  public static int ParseTrack(Embed embed)
  {
    EmbedField progressField = embed.Fields.FirstOrDefault(field => Regex.IsMatch(field.Name, ProgressFieldPattern));
    return ParseTrack(progressField);
  }
  public static List<IEmote> TicksToEmojiList(int ticks)
  {
    int score = ITrack.GetScore(ticks);
    List<IEmote> emojis = Enumerable.Repeat(TickEmoji[BoxSize], score).ToList();
    int remainder = ticks % BoxSize;
    if (remainder > 0)
    {
      emojis.Add(TickEmoji[remainder]);
    }
    return emojis;
  }
  public static string TicksToEmojiTrack(int ticks)
  {
    List<IEmote> emoji = TicksToEmojiList(ticks);
    int padding = TrackSize;
    if (emoji?.Count > 0)
    {
      padding -= emoji.Count;
    }
    emoji.AddRange(
      Enumerable.Repeat(
        TickEmoji[0],
        padding
    ));
    string result = string.Join(" ", emoji);
    result += MobileEmojiSizer;
    return result;
  }
  protected static readonly List<IEmote> TickEmoji = new()
  {
    Emote.Parse("<:progress0:880599822468534374>"),
    Emote.Parse("<:progress1:880599822736965702>"),
    Emote.Parse("<:progress2:880599822724390922>"),
    Emote.Parse("<:progress3:880599822736957470>"),
    Emote.Parse("<:progress4:880599822820864060>")
  };

  protected static readonly Dictionary<string, IEmote> Emoji = new()
  {
    { "roll", new Emoji("🎲") },
    { "recommit", new Emoji("🔄") },
    { "reference", new Emoji("📖") }
  };
  public const string DefaultTrackName = "Track";
  protected static EmbedFieldBuilder TrackField(int ticks = 0, string name = DefaultTrackName)
  {
    int score = ticks / BoxSize;
    return new EmbedFieldBuilder()
      .WithName($"{name} [{score}/{TrackSize}]")
      .WithValue(ITrack.TicksToEmojiTrack(ticks));
  }

  /// <summary>
  /// Renders a text description of a progress amount, for instance: "1 box", "2 ticks", "2 boxes and 2 ticks"
  /// </summary>
  public static string TickString(int ticks)
  {
    string tickAutoPlural = "tick";
    if (ticks >= 3)
    {
      int boxes = ticks / BoxSize;
      int remainder = ticks % BoxSize;
      string result = boxes.ToString() + " " + (boxes > 1 ? "boxes" : "box");
      if (remainder > 0)
      {
        if (remainder > 1) { tickAutoPlural += "s"; }
        result += $" and {remainder} {tickAutoPlural}";
      }
      return result;
    }
    if (ticks > 1) { tickAutoPlural += "s"; }
    return $"{ticks} {tickAutoPlural}";
  }
  public static EmbedFieldBuilder StrikeField(EmbedFieldBuilder field)
  {
    field = StrikeFieldName(field);
    field = StrikeFieldValue(field);
    return field;
  }
  public static EmbedFieldBuilder StrikeFieldName(EmbedFieldBuilder field)
  {
    field.Name = "~~" + field.Name + "~~";
    return field;
  }
  public static EmbedFieldBuilder StrikeFieldValue(EmbedFieldBuilder field)
  {
    field.Value = "~~" + field.Value + "~~";
    return field;
  }
}