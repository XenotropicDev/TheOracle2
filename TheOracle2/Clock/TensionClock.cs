namespace TheOracle2.GameObjects;

public class TensionClock : Clock
{
  public TensionClock(Embed embed) : base(embed) { }
  public TensionClock(EmbedField embedField) : base(embedField) { }
  public TensionClock(ClockSize segments, int filledSegments, string text) : base(segments, filledSegments, text) { }
  public override string ClockType => "Tension Clock";
  public override string FillMessage => "The threat or deadline triggers. This should result in harrowing problems for your character. It may even force you to abandon an expedition, fight, vow, or other challenge.";
}