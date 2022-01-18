namespace TheOracle2.GameObjects;

public class TensionClock : Clock
{
    public TensionClock(Embed embed, bool alerts = false) : base(embed, alerts)
    {
    }

    public TensionClock(ClockSize segments, int filledSegments, string title, string description = "", bool alerts = false) : base(segments, filledSegments, title, description, alerts)
    {
    }

    public override string EmbedCategory => "Tension Clock";
    public override string LogMessage => "The threat or deadline triggers. This should result in harrowing problems for your character. It may even force you to abandon an expedition, fight, vow, or other challenge.";
}