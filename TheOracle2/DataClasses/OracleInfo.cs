using Newtonsoft.Json;
using OracleData;

namespace TheOracle2.DataClasses
{
    public class AddTemplate
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        public Attributes Attributes { get; set; }

        [JsonProperty(PropertyName = "Template type")]
        public string Templatetype { get; set; }
    }

    public partial class Attributes
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Derelict Type")]
        public string DerelictType { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public IList<string> Location { get; set; }
    }

    public partial class ChanceTable
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Add template")]
        public AddTemplate Addtemplate { get; set; }

        public IList<string> Assets { get; set; }
        public int Chance { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }

        [JsonProperty(PropertyName = "Game object")]
        public GameObject Gameobject { get; set; }

        [JsonProperty(PropertyName = "Multiple rolls")]
        public MultipleRolls Multiplerolls { get; set; }

        public List<Oracle> Oracles { get; set; }
        public List<Suggest> Suggest { get; set; }
        public string Thumbnail { get; set; }
        public int Value { get; set; }
    }

    public partial class GameObject
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        public int Amount { get; set; }
        public Attributes Attributes { get; set; }

        [JsonProperty(PropertyName = "Object type")]
        public string Objecttype { get; set; }
    }

    public class Inherit
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        public string Category { get; set; }
        public IList<string> Exclude { get; set; }
        public IList<string> Name { get; set; }
        public Requires Requires { get; set; }
    }

    public class MultipleRolls
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Allow duplicates")]
        public bool Allowduplicates { get; set; }

        public int Amount { get; set; }
    }

    public partial class Oracle
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        public IList<string> Aliases { get; set; }

        [JsonProperty(PropertyName = "Allow duplicate rolls")]
        public bool AllowDuplicateRolls { get; set; }

        public string Category { get; set; }
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string DisplayName { get; set; }

        public bool Initial { get; set; }

        [JsonProperty(PropertyName = "Max rolls")]
        public int Maxrolls { get; set; }

        [JsonProperty(PropertyName = "Min rolls")]
        public int Minrolls { get; set; }

        public string Name { get; set; }

        [JsonProperty(PropertyName = "Oracle type")]
        public string OracleType { get; set; }

        public bool Repeatable { get; set; }
        public Requires Requires { get; set; }

        [JsonProperty(PropertyName = "Select table by")]
        public string SelectTableBy { get; set; }

        public string Subgroup { get; set; }
        public List<ChanceTable> Table { get; set; }
        public List<Tables> Tables { get; set; }

        [JsonProperty(PropertyName = "Use with")]
        public List<UseWith> UseWith { get; set; }

        [JsonProperty(PropertyName = "Part of speech")]
        public IList<string> PartOfSpeech { get; set; }

        [JsonProperty(PropertyName = "Content tags")]
        public IList<string> ContentTags { get; set; }

        public string Group { get; set; }
    }

    public class OracleInfo
    {
        public OracleInfo()
        {
        }

        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        public IList<string> Aliases { get; set; }
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string DisplayName { get; set; }

        public List<Inherit> Inherits { get; set; }
        public string Name { get; set; }
        public List<Oracle> Oracles { get; set; }
        public Source Source { get; set; }
        public List<Subcategory> Subcategories { get; set; }
        public IList<string> Tags { get; set; }
    }

    public partial class Requires
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Derelict Type")]
        public IList<string> DerelictType { get; set; }

        public IList<string> Environment { get; set; }
        public IList<string> Life { get; set; }
        public IList<string> Location { get; set; }

        [JsonProperty(PropertyName = "Planetary Class")]
        public IList<string> PlanetaryClass { get; set; }

        public IList<string> Region { get; set; }
        public IList<string> Scale { get; set; }

        [JsonProperty(PropertyName = "Starship Type")]
        public IList<string> StarshipType { get; set; }

        [JsonProperty(PropertyName = "Theme Type")]
        public IList<string> ThemeType { get; set; }

        public IList<string> Type { get; set; }

        public IList<string> Zone { get; set; }
    }

    public class Rootobject
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        public List<OracleInfo> OracleInfos { get; set; }
    }

    public class Subcategory
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        public IList<string> Aliases { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string Displayname { get; set; }

        public List<Inherit> Inherits { get; set; }
        public string Name { get; set; }
        public List<Oracle> Oracles { get; set; }
        public Requires Requires { get; set; }

        [JsonProperty(PropertyName = "Sample Names")]
        public IList<string> SampleNames { get; set; }

        public Source Source { get; set; }
        public string Thumbnail { get; set; }
    }

    public class Suggest
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Game object")]
        public GameObject Gameobject { get; set; }

        public List<Oracle> Oracles { get; set; }
    }

    public class Tables
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        public IList<string> Aliases { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string Displayname { get; set; }

        public string Name { get; set; }
        public Requires Requires { get; set; }
        public List<ChanceTable> Table { get; set; }
    }

    public class UseWith
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }

        public string Category { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
    }
}