using OracleData;
using System.ComponentModel.DataAnnotations;

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

    [JsonPropertyName("Template type")]
    [Newtonsoft.Json.JsonProperty("Template type")]
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

    [JsonPropertyName("Derelict Type")]
    [Newtonsoft.Json.JsonProperty("Derelict Type")]
    public string DerelictType { get; set; }

    public IList<string> Location { get; set; }
    public string Type { get; set; }
}

public partial class ChanceTable : IRollResult
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

    [JsonPropertyName("Add template")]
    [Newtonsoft.Json.JsonProperty("Add template")]
    public virtual AddTemplate Addtemplate { get; set; }

    public IList<string> Assets { get; set; }
    public int Chance { get; set; }
    public string Description { get; set; }
    public string Details { get; set; }

    [JsonPropertyName("Game object")]
    [Newtonsoft.Json.JsonProperty("Game object")]
    public virtual GameObject Gameobject { get; set; }

    [JsonPropertyName("Multiple rolls")]
    [Newtonsoft.Json.JsonProperty("Multiple rolls")]
    public virtual MultipleRolls Multiplerolls { get; set; }

    public virtual List<OracleStub> Oracles { get; set; }

    [JsonPropertyName("Part of speech")]
    [Newtonsoft.Json.JsonProperty("Part of speech")]
    public IList<string> PartOfSpeech { get; set; }
    public virtual List<Suggest> Suggest { get; set; }
    public string Thumbnail { get; set; }
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

    [JsonPropertyName("Object type")]
    [Newtonsoft.Json.JsonProperty("Object type")]
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
    public virtual Requires Requires { get; set; }
}

public class MultipleRolls
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int ChanceTableId { get; set; }
    [JsonIgnore]
    public virtual ChanceTable ChanceTable { get; set; }

    [JsonPropertyName("Allow duplicates")]
    [Newtonsoft.Json.JsonProperty("Allow duplicates")]
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

    [JsonPropertyName("Allow duplicate rolls")]
    [Newtonsoft.Json.JsonProperty("Allow duplicate rolls")]
    public bool AllowDuplicateRolls { get; set; }

    public string Category { get => OracleInfo.Name; }
    public string Description { get; set; }

    [JsonPropertyName("Display name")]
    [Newtonsoft.Json.JsonProperty("Display name")]
    public string DisplayName { get; set; }

    public bool Initial { get; set; }

    [JsonPropertyName("Max rolls")]
    [Newtonsoft.Json.JsonProperty("Max rolls")]
    public int Maxrolls { get; set; }

    [JsonPropertyName("Min rolls")]
    [Newtonsoft.Json.JsonProperty("Min rolls")]
    public int Minrolls { get; set; }

    public string Name { get; set; }

    [JsonPropertyName("Oracle type")]
    [Newtonsoft.Json.JsonProperty("Oracle type")]
    public string OracleType { get; set; }

    public bool Repeatable { get; set; }
    public virtual Requires Requires { get; set; }

    [JsonPropertyName("Select table by")]
    [Newtonsoft.Json.JsonProperty("Select table by")]
    public string SelectTableBy { get; set; }

    public string Subgroup { get; set; }
    public virtual List<ChanceTable> Table { get; set; }
    public virtual List<Tables> Tables { get; set; }

    [JsonPropertyName("Use with")]
    [Newtonsoft.Json.JsonProperty("Use with")]
    public virtual List<UseWith> UseWith { get; set; }

    [JsonPropertyName("Part of speech")]
    [Newtonsoft.Json.JsonProperty("Part of speech")]
    public IList<string> PartOfSpeech { get; set; }

    [JsonPropertyName("Content tags")]
    [Newtonsoft.Json.JsonProperty("Content tags")]
    public IList<string> ContentTags { get; set; }

    public string Group { get; set; }

}

public class OracleInfo
{
    [JsonIgnore]
    public int Id { get; set; }

    public IList<string> Aliases { get; set; }
    public string Description { get; set; }

    [JsonPropertyName("Display name")]
    [Newtonsoft.Json.JsonProperty("Display name")]
    public string DisplayName { get; set; }

    public virtual List<Inherit> Inherits { get; set; }
    public string Name { get; set; }
    public virtual List<Oracle> Oracles { get; set; }
    public virtual Source Source { get; set; }
    public virtual List<Subcategory> Subcategories { get; set; }
    public IList<string> Tags { get; set; }
}

public partial record Requires
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int? InheritId { get; set; }
    [JsonIgnore]
    public virtual Inherit Inherit { get; set; }

    [JsonIgnore]
    public int? OracleId { get; set; }
    [JsonIgnore]
    public virtual Oracle Oracle { get; set; }

    [JsonIgnore]
    public int? TablesId { get; set; }
    [JsonIgnore]
    public virtual Tables Tables { get; set; }

    [JsonPropertyName("Derelict Type")]
    [Newtonsoft.Json.JsonProperty("Derelict Type")]
    public IList<string> DerelictType { get; set; }

    public IList<string> Environment { get; set; }
    public IList<string> Life { get; set; }
    public IList<string> Location { get; set; }

    [JsonPropertyName("Planetary Class")]
    [Newtonsoft.Json.JsonProperty("Planetary Class")]
    public IList<string> PlanetaryClass { get; set; }

    public IList<string> Region { get; set; }
    public IList<string> Scale { get; set; }

    [JsonPropertyName("Starship Type")]
    [Newtonsoft.Json.JsonProperty("Starship Type")]
    public IList<string> StarshipType { get; set; }

    [JsonPropertyName("Theme Type")]
    [Newtonsoft.Json.JsonProperty("Theme Type")]
    public IList<string> ThemeType { get; set; }

    public IList<string> Type { get; set; }

    public IList<string> Zone { get; set; }
}

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
    public string Description { get; set; }

    [JsonPropertyName("Display name")]
    [Newtonsoft.Json.JsonProperty("Display name")]
    public string Displayname { get; set; }

    public virtual List<Inherit> Inherits { get; set; }
    public string Name { get; set; }
    public virtual List<Oracle> Oracles { get; set; }
    public virtual Requires Requires { get; set; }

    [JsonPropertyName("Sample Names")]
    [Newtonsoft.Json.JsonProperty("Sample Names")]
    public IList<string> SampleNames { get; set; }

    public virtual Source Source { get; set; }
    public string Thumbnail { get; set; }
}

public class Suggest
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonIgnore]
    public int ChanceTableId { get; set; }
    [JsonIgnore]
    public virtual ChanceTable ChanceTable { get; set; }

    [JsonPropertyName("Game object")]
    [Newtonsoft.Json.JsonProperty("Game object")]
    public virtual GameObject Gameobject { get; set; }
    public virtual List<OracleStub> Oracles { get; set; }
    
    [JsonPropertyName("Location Theme")]
    [Newtonsoft.Json.JsonProperty("Location Theme")]
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

    [JsonPropertyName("Display name")]
    [Newtonsoft.Json.JsonProperty("Display name")]
    public string Displayname { get; set; }

    public string Name { get; set; }
    public virtual Requires Requires { get; set; }
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