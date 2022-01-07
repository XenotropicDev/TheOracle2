namespace TheOracle2.GameObjects;

public class TensionClock : Clock
{
  public TensionClock(Embed embed) : base(embed) { }
  public TensionClock(EmbedField embedField) : base(embedField) { }
  public TensionClock(ClockSize segments, int filledSegments, string text) : base(segments, filledSegments, text) { }
  public override string ClockType { get => "Tension Clock"; }
}