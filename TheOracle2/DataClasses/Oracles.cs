namespace TheOracle2.DataClasses;

public class TableDisplay : Display
{
    public Table Table { get; set; }

    [JsonProperty("Column of")]
    public string ColumnOf { get; set; }
}

public class ResultColumn
{
    public string Label { get; set; }

    [JsonProperty("Use content from")]
    public string UseContentFrom { get; set; }
    public string Key { get; set; }
}

public class RollColumn
{
    public string Label { get; set; }

    [JsonProperty("Use content from")]
    public string UseContentFrom { get; set; }
}

public class Table
{
    [JsonProperty("Result columns")]
    public List<ResultColumn> ResultColumns { get; set; }

    [JsonProperty("Roll columns")]
    public List<RollColumn> RollColumns { get; set; }
    public int Floor { get; set; }
    public int Ceiling { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }
    public string Result { get; set; }
    public string Summary { get; set; }
    public Suggestions Suggestions { get; set; }

    [JsonProperty("Oracle rolls")]
    public List<string> OracleRolls { get; set; }

    [JsonProperty("Multiple rolls")]
    public MultipleRolls MultipleRolls { get; set; }
    public List<Attribute> Attributes { get; set; }

    [JsonProperty("Roll template")]
    public RollTemplate RollTemplate { get; set; }

    [JsonProperty("Game objects")]
    public List<GameObject> GameObjects { get; set; }
    public TableDisplay Display { get; set; }
    public Content Content { get; set; }
}

public class Content
{
    [JsonProperty("Part of speech")]
    public List<string> PartOfSpeech { get; set; }
    public List<string> Tags { get; set; }
}

public class Attribute
{
    public string Key { get; set; }
    public List<string> Values { get; set; }
    public string Value { get; set; }
}

public class Requires
{
    public List<Attribute> Attributes { get; set; }
}

public class GameObject
{
    [JsonProperty("Object type")]
    public string ObjectType { get; set; }
    public Requires Requires { get; set; }
}

public class Suggestions
{
    public List<string> Assets { get; set; }

    [JsonProperty("Game objects")]
    public List<GameObject> GameObjects { get; set; }

    [JsonProperty("Oracle rolls")]
    public List<string> OracleRolls { get; set; }
}

public class MultipleRolls
{
    public int Amount { get; set; }

    [JsonProperty("Allow duplicates")]
    public bool AllowDuplicates { get; set; }

    [JsonProperty("Make it worse")]
    public bool MakeItWorse { get; set; }
}

public class RollTemplate
{
    public string Result { get; set; }
    public string Description { get; set; }

}

public class SetsAttribute
{
    public string Key { get; set; }
}

public class Oracle
{
    public Source Source { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }
    public string Name { get; set; }

    [JsonProperty("Member of")]
    public string MemberOf { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public TableDisplay Display { get; set; }
    public Usage Usage { get; set; }
    public Content Content { get; set; }
    public List<Table> Table { get; set; }
    public List<string> Aliases { get; set; }
    public List<Oracle> Oracles { get; set; }
}

public class Category
{
    public Source Source { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }
    public string Name { get; set; }
    public TableDisplay Display { get; set; }
    
    [JsonProperty("Category")]
    public string CategoryParentId { get; set; }
    public Usage Usage { get; set; }
    public List<Oracle> Oracles { get; set; }
    public string Description { get; set; }
    public List<string> Aliases { get; set; }

    [JsonProperty("Sample Names")]
    public List<string> SampleNames { get; set; }
}

public class OracleInfo
{
    public Source Source { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }
    public string Name { get; set; }
    public List<string> Aliases { get; set; }
    public TableDisplay Display { get; set; }
    public List<Oracle> Oracles { get; set; }
    public string Description { get; set; }
    public List<Category> Categories { get; set; }
}

