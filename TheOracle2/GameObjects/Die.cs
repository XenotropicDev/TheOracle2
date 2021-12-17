namespace TheOracle2.GameObjects;

public class Die
{
    public Die(int sides)
    {
        if (sides <= 0) throw new ArgumentOutOfRangeException(nameof(sides));
        Sides = sides;
    }

    public int Sides { get; }

    public int Roll()
    {
        return BotRandom.Instance.Next(1, Sides + 1);
    }
}
