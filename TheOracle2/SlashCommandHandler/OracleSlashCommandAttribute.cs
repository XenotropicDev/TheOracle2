namespace TheOracle2;

public class OracleSlashCommandAttribute : Attribute
{
    private string name;

    public string Name { get => name; private set => name = value.ToLower(); }

    public OracleSlashCommandAttribute(string name)
    {
        Name = name;
    }
}