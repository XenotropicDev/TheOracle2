using OracleData;

namespace TheOracle2.DataClasses;

//Todo: convert the classes in this file to records that should be.
//lazy loading virtual regex: public (?!class|int|string|partial|.List<string>|virtual|.*?\()

public class AddTemplate
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int ChanceTableId { get; set; }

    [JsonIgnore]
    public virtual ChanceTable ChanceTable { get; set; }

    public virtual Attributes Attributes { get; set; }

    [JsonProperty("Template type")]
    public string Templatetype { get; set; }
}

public partial class Attributes
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int? AddTemplateId { get; set; }

    [JsonIgnore]
    public virtual AddTemplate AddTemplate { get; set; }

    [JsonIgnore]
    public int? GameObjectId { get; set; }

    [JsonIgnore]
    public virtual GameObject GameObject { get; set; }

    [JsonProperty("Derelict Type")]
    public IList<string> DerelictType { get; set; }

    public IList<string> Location { get; set; }
    public IList<string> Type { get; set; }
}

public partial class ChanceTable
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int? OracleId { get; set; }

    [JsonIgnore]
    public virtual Oracle Oracle { get; set; }

    [JsonIgnore]
    public int? TableId { get; set; }

    [JsonIgnore]
    public virtual Tables Tables { get; set; }

    [JsonProperty("Add template")]
    public virtual AddTemplate Addtemplate { get; set; }

    public int Amount { get; set; }
    public IList<string> Assets { get; set; }
    public int Chance { get; set; }
    public string Description { get; set; }
    public string Details { get; set; }

    [JsonProperty("Game objects")]
    public virtual List<GameObject> Gameobject { get; set; }

    [JsonProperty("Multiple rolls")]
    public virtual MultipleRolls Multiplerolls { get; set; }

    public virtual List<OracleStub> Oracles { get; set; }

    [JsonProperty("Part of speech")]
    public IList<string> PartOfSpeech { get; set; }

    public virtual Suggest Suggestions { get; set; }
    [JsonProperty("Thumbnail")]
    public string Image { get; set; }
    public int Value { get; set; }
}

public class OracleStub
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int? ChanceTableId { get; set; }

    [JsonIgnore]
    public virtual ChanceTable ChanceTable { get; set; }

    [JsonIgnore]
    public int? SuggestId { get; set; }

    [JsonIgnore]
    public virtual Suggest Suggest { get; set; }

    public string Category { get; set; }
    public string Name { get; set; }
    public string Subcategory { get; set; }
}

public partial class GameObject
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int? ChanceTableId { get; set; }

    [JsonIgnore]
    public virtual ChanceTable ChanceTable { get; set; }

    [JsonIgnore]
    public int? SuggestId { get; set; }

    [JsonIgnore]
    public virtual Suggest Suggest { get; set; }

    public int Amount { get; set; }
    public virtual Attributes Attributes { get; set; }

    [JsonProperty("Object type")]
    public string Objecttype { get; set; }
}

public class Inherit
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int? OracleInfoId { get; set; }

    [JsonIgnore]
    public virtual OracleInfo OracleInfo { get; set; }

    [JsonIgnore]
    public int? SubcategoryId { get; set; }

    [JsonIgnore]
    public virtual Subcategory Subcategory { get; set; }

    public string Category { get; set; }
    public IList<string> Exclude { get; set; }
    public IList<string> Name { get; set; }
    public IDictionary<string, string[]> Requires { get; set; }
}

public class MultipleRolls
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int ChanceTableId { get; set; }

    [JsonIgnore]
    public virtual ChanceTable ChanceTable { get; set; }

    [JsonProperty("Allow duplicates")]
    public bool Allowduplicates { get; set; }

    public int Amount { get; set; }
}

public partial class Oracle
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int? OracleInfoId { get; set; }

    [JsonIgnore]
    public virtual OracleInfo OracleInfo { get; set; }

    [JsonIgnore]
    public int? SubcategoryId { get; set; }

    [JsonIgnore]
    public virtual Subcategory Subcategory { get; set; }

    public IList<string> Aliases { get; set; }

    [JsonProperty("Allow duplicate rolls")]
    public bool AllowDuplicateRolls { get; set; }

    public string Category { get => OracleInfo?.Name; }
    public string Description { get; set; }

    [JsonProperty("Display name")]
    public string DisplayName { get; set; }

    public bool Initial { get; set; }

    [JsonProperty("Max rolls")]
    public int Maxrolls { get; set; }

    [JsonProperty("Min rolls")]
    public int Minrolls { get; set; }

    public string Name { get; set; }

    [JsonProperty("Oracle type")]
    public string OracleType { get; set; }

    public bool Repeatable { get; set; }
    public IDictionary<string, string[]> Requires { get; set; }

    [JsonProperty("Select table by")]
    public string SelectTableBy { get; set; }

    public virtual Source Source { get; set; }
    public string Subgroup { get; set; }
    public virtual List<ChanceTable> Table { get; set; }
    public virtual List<Tables> Tables { get; set; }

    [JsonProperty("Use with")]
    public virtual List<UseWith> UseWith { get; set; }

    [JsonProperty("Part of speech")]
    public IList<string> PartOfSpeech { get; set; }

    [JsonProperty("Content tags")]
    public IList<string> ContentTags { get; set; }

    public string Group { get; set; }
}

public class OracleInfo
{
    [JsonIgnore]
    public int Id { get; set; }

    public IList<string> Aliases { get; set; }
    public string Description { get; set; }

    [JsonProperty("Display name")]
    public string DisplayName { get; set; }

    public virtual List<Inherit> Inherits { get; set; }
    public string Name { get; set; }
    public virtual List<Oracle> Oracles { get; set; }
    public virtual Source Source { get; set; }
    public virtual List<Subcategory> Subcategories { get; set; }

    [JsonProperty("Content tags")]
    public IList<string> Tags { get; set; }
}

//public partial record Requires
//{
//    [JsonIgnore]
//    public int Id { get; set; }

//    [JsonIgnore]
//    public int? InheritId { get; set; }
//    [JsonIgnore]
//    public virtual Inherit Inherit { get; set; }

//    [JsonIgnore]
//    public int? OracleId { get; set; }
//    [JsonIgnore]
//    public virtual Oracle Oracle { get; set; }

//    [JsonIgnore]
//    public int? TablesId { get; set; }
//    [JsonIgnore]
//    public virtual Tables Tables { get; set; }

//    [JsonProperty("Derelict Type")]
//    public IList<string> DerelictType { get; set; }

//    public IList<string> Environment { get; set; }
//    public IList<string> Life { get; set; }
//    public IList<string> Location { get; set; }

//    [JsonProperty("Planetary Class")]
//    public IList<string> PlanetaryClass { get; set; }

//    public IList<string> Region { get; set; }
//    public IList<string> Scale { get; set; }

//    [JsonProperty("Starship Type")]
//    public IList<string> StarshipType { get; set; }

//    [JsonProperty("Theme Type")]
//    public IList<string> ThemeType { get; set; }

//    public IList<string> Type { get; set; }

//    public IList<string> Zone { get; set; }
//}

public class Rootobject
{
    [JsonIgnore]
    public int Id { get; set; }

    public virtual List<OracleInfo> OracleInfos { get; set; }
}

public class Subcategory
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int OracleInfoId { get; set; }

    [JsonIgnore]
    public virtual OracleInfo OracleInfo { get; set; }

    public IList<string> Aliases { get; set; }
    public string Category { get; set; }

    [JsonProperty("Content tags")]
    public IList<string> ContentTags { get; set; }

    public string Description { get; set; }

    [JsonProperty("Display name")]
    public string Displayname { get; set; }

    public virtual List<Inherit> Inherits { get; set; }
    public string Name { get; set; }
    public virtual List<Oracle> Oracles { get; set; }
    public IDictionary<string, string[]> Requires { get; set; }

    [JsonProperty("Sample Names")]
    public IList<string> SampleNames { get; set; }

    public virtual Source Source { get; set; }

    [JsonProperty("Subcategory of")]
    public string SubcategoryOf { get; set; }

    [JsonProperty("Thumbnail")]
    public string Image { get; set; }
}

public class Suggest
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int ChanceTableId { get; set; }

    [JsonIgnore]
    public virtual ChanceTable ChanceTable { get; set; }

    [JsonProperty("Game objects")]
    public virtual List<GameObject> Gameobject { get; set; }

    public virtual List<OracleStub> Oracles { get; set; }

    [JsonProperty("Location Theme")]
    public string LocationTheme { get; set; }
}

public class Tables
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int OracleId { get; set; }

    [JsonIgnore]
    public virtual Oracle Oracle { get; set; }

    public IList<string> Aliases { get; set; }

    [JsonProperty("Display name")]
    public string Displayname { get; set; }

    public string Name { get; set; }
    public IDictionary<string, string[]> Requires { get; set; }
    public virtual List<ChanceTable> Table { get; set; }
}

public class UseWith
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int OracleId { get; set; }

    [JsonIgnore]
    public virtual Oracle Oracle { get; set; }

    public string Category { get; set; }
    public string Name { get; set; }
    public string Group { get; set; }
}