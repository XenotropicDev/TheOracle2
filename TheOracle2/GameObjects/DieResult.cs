namespace TheOracle2.GameObjects;

public class DieResult
{
    public DieResult(Die die)
    {
        Die = die;
        RollValue = die.Roll();
    }

    public DieResult(Die die, int rollValue)
    {
        Die = die;
        RollValue = rollValue;
    }

    public Die Die { get; }
    public int RollValue { get; }

    public void ChangeResult()
    {
        
    }

    public override string ToString()
    {
        return RollValue.ToString();
    }
}
