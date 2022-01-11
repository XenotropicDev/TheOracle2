namespace TheOracle2.GameObjects;
public interface IClock
{
  public string ToString() { return $"{Filled}/{Segments}"; }

  public EmbedBuilder ToEmbed();
  public EmbedFieldBuilder ToEmbedField();
  public EmbedBuilder AlertEmbed();
  public string FillMessage { get; set; }
  public int Segments { get; }
  public int Filled { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public string EmbedCategory { get; }
  public bool IsFull => Filled >= Segments;
  public static string AdvanceLabel => "Advance Clock";
  public ComponentBuilder MakeComponents();
  public static EmbedBuilder ToEmbedStub(string embedCategory, string title, int segments, int filled)
  {
    return new EmbedBuilder()
    .WithAuthor(embedCategory)
    .WithTitle(title)
    .WithThumbnailUrl(
      IClock.Images[segments][filled])
    .WithColor(
      IClock.ColorRamp[segments][filled]);
  }
  public static Tuple<int, int> Parseclock(Embed embed)
  {
    EmbedField clockField = embed.Fields.FirstOrDefault(field => field.Name == "Clock");
    string[] valueStrings = clockField.Value.Split("/");
    int[] values = valueStrings.Select(value => int.Parse(value)).ToArray();
    return new Tuple<int, int>(values[0], values[1]);
  }
  public static readonly Dictionary<string, Emoji> UxEmoji = new()
  {
    { "reset", new Emoji("↩️") }
  };
  public static readonly Dictionary<int, Emoji> OddsEmoji = new()
  {
    { 10, new Emoji("🕐") },
    { 25, new Emoji("🕒") },
    { 50, new Emoji("🕧") },
    { 75, new Emoji("🕘") },
    { 90, new Emoji("🕚") },
    { 100, new Emoji("🕛") }
  };

  public static readonly Dictionary<int, Color[]> ColorRamp = new()
  {
    {
      4,
      new Color[] { new Color(0x47aedd), new Color(0x5377cb), new Color(0x842a8c), new Color(0xb30065), new Color(0xc50933) }
    },
    {
      6,
      new Color[] { new Color(0x47aedd), new Color(0x428bd5), new Color(0x6661bb), new Color(0x842a8c), new Color(0xa70874), new Color(0xbd0055), new Color(0xc50933) }
    },
    {
      8,
      new Color[] { new Color(0x47aedd), new Color(0x3d94d8), new Color(0x5377cb), new Color(0x6f55b2), new Color(0x842a8c), new Color(0x9f147b), new Color(0xb30065), new Color(0xc0004d), new Color(0xc50933) }
    },
    {
      10,
      new Color[] { new Color(0x47aedd), new Color(0x3c99da), new Color(0x4883d1), new Color(0x5f6ac2), new Color(0x744eab), new Color(0x842a8c), new Color(0x9a197f), new Color(0xac006e), new Color(0xb9005c), new Color(0xc20048), new Color(0xc50933) }
    }
  };
  public static readonly Dictionary<int, string[]> Images = new()
  {
    {
      4,
      new string[]{
        "https://i.imgur.com/Ahbi1DV.png",
        "https://i.imgur.com/7RTHuPB.png",
        "https://i.imgur.com/a0hlbCn.png",
        "https://i.imgur.com/A3e5aHC.png",
        "https://i.imgur.com/8DzJNyy.png",
      }
    },
    {
      6,
      new string[]{
      "https://i.imgur.com/rvDLRZO.png",
      "https://i.imgur.com/EZbNNRC.png",
      "https://i.imgur.com/RlaRzgz.png",
      "https://i.imgur.com/OJ2WVSR.png",
      "https://i.imgur.com/YgLlojT.png",
      "https://i.imgur.com/ZpyIpTC.png",
      "https://i.imgur.com/KjYg7aC.png"
      }
    },
    {
      8,
      new string[]{
      "https://i.imgur.com/Qi0pkYD.png",
      "https://i.imgur.com/9wwW3Sh.png",
      "https://i.imgur.com/sM27Mbf.png",
      "https://i.imgur.com/GsKElZv.png",
      "https://i.imgur.com/QvYHujk.png",
      "https://i.imgur.com/nNEFyRr.png",
      "https://i.imgur.com/a9Qvkin.png",
      "https://i.imgur.com/0dclJ9Q.png",
      "https://i.imgur.com/5Z0bQ9K.png",
    }
    },
    {
      10,
      new string[]{
      "https://i.imgur.com/u9erdAx.png",
      "https://i.imgur.com/0zfi1PJ.png",
      "https://i.imgur.com/ayanbMK.png",
      "https://i.imgur.com/OtfwmEf.png",
      "https://i.imgur.com/uUWoyZV.png",
      "https://i.imgur.com/eKhfGoj.png",
      "https://i.imgur.com/cwzEkCD.png",
      "https://i.imgur.com/GNJPzru.png",
      "https://i.imgur.com/cMQNAZV.png",
      "https://i.imgur.com/EaOxmdt.png",
      "https://i.imgur.com/NRhTaBR.png",
    }
    }
  };
  public static IClock FromEmbed(Embed embed)
  {
    return embed.Author.ToString() switch
    {
      "Campaign Clock" => new CampaignClock(embed),
      "Tension Clock" => new TensionClock(embed),
      "Scene Challenge" => new SceneChallenge(embed),
      _ => throw new ArgumentOutOfRangeException(nameof(embed), "Embed must be a 'Campaign Clock', 'Tension Clock', or 'Scene Challenge'"),
    };
  }
}