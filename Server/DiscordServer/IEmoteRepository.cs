namespace TheOracle2;

public interface IEmoteRepository
{
    IEmote MarkProgress { get; }
    IEmote Recommit { get; }
    IEmote Reference { get; }
    IEmote Roll { get; }
    IEmote[] ProgressEmotes { get; }
}

public class HardCodedEmoteRepo : IEmoteRepository
{
    internal static readonly Dictionary<string, IEmote> EmojiLibrary = new()
    {
        { "roll", new Emoji("🎲") },
        { "recommit", new Emoji("🔄") },
        { "reference", new Emoji("📖") },
        { "progress", Emote.Parse("<:progress4:880599822820864060>") }
    };

    public HardCodedEmoteRepo()
    {
        Roll = new Emoji("🎲");
        Recommit = new Emoji("🔄");
        Reference = new Emoji("📖");

        MarkProgress = Emote.TryParse("<:progress4:880599822820864060>", out var progress) ? progress : new Emoji("☑️");

        ProgressEmotes = new List<IEmote>
        {
            Emote.TryParse("<:progress0:880599822468534374>", out var progress0) ? progress0 : new Emoji("🟦"),
            Emote.TryParse("<:progress1:880599822736965702>", out var progress1) ? progress1 : new Emoji("🇮"),
            Emote.TryParse("<:progress2:880599822724390922>", out var progress2) ? progress2 : new Emoji("🇽"),
            Emote.TryParse("<:progress3:880599822736957470>", out var progress3) ? progress3 : new Emoji("*️⃣"),
            Emote.TryParse("<:progress4:880599822820864060>", out var progress4) ? progress4 : new Emoji("#️⃣")
        }.ToArray();
    }

    public IEmote Roll { get; }

    public IEmote MarkProgress { get; }

    public IEmote Recommit { get; }

    public IEmote Reference { get; }

    public IEmote[] ProgressEmotes { get; }
}
