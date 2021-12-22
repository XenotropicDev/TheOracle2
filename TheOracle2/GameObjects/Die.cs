namespace TheOracle2.GameObjects;

public class Die
{
    private readonly Random random;

    public Die(int sides, Random random)
    {
        if (sides <= 0) throw new ArgumentOutOfRangeException(nameof(sides));
        Sides = sides;
        this.random = random;
    }

    public int Sides { get; }

    public int Roll()
    {
        return random.Next(1, Sides + 1);
    }
}
