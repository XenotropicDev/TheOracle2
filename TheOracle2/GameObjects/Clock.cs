namespace TheOracle2.GameObjects;

public class Clock
{
  Clock(ClockSize segments = (ClockSize)6, int filledSegments = 0)
  {
    if (filledSegments < 0 || filledSegments > ((int)segments))
    {
      throw new ArgumentOutOfRangeException("filledSegments must be an integer no less than 0, and no greater than the number of segments.");
    }
    Segments = (int)segments;
    FilledSegments = filledSegments;
  }
  int Segments { get; set; }
  int FilledSegments { get; set; }
}