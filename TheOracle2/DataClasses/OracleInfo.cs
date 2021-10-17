using Newtonsoft.Json;
using System.Collections.Generic;
using TheOracle2.DataClasses;

namespace TheOracle2
{
    public class AddTemplate
    {
        public Attributes Attributes { get; set; }

        [JsonProperty(PropertyName = "Template type")]
        public string Templatetype { get; set; }
    }

    public partial class Attributes
    {
        [JsonProperty(PropertyName = "Derelict Type")]
        public string DerelictType { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Location { get; set; }
    }

    public partial class ChanceTable
    {
        [JsonProperty(PropertyName = "Add template")]
        public AddTemplate Addtemplate { get; set; }

        public string[] Assets { get; set; }
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
        public int Amount { get; set; }
        public Attributes Attributes { get; set; }

        [JsonProperty(PropertyName = "Object type")]
        public string Objecttype { get; set; }
    }

    public class Inherit
    {
        public string Category { get; set; }
        public string[] Exclude { get; set; }
        public string[] Name { get; set; }
        public Requires Requires { get; set; }
    }

    public class MultipleRolls
    {
        [JsonProperty(PropertyName = "Allow duplicates")]
        public bool Allowduplicates { get; set; }

        public int Amount { get; set; }
    }

    public partial class Oracle
    {
        public string[] Aliases { get; set; }

        [JsonProperty(PropertyName = "Allow duplicate rolls")]
        public bool Allowduplicaterolls { get; set; }

        public string Category { get; set; }
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string Displayname { get; set; }

        public bool Initial { get; set; }

        [JsonProperty(PropertyName = "Max rolls")]
        public int Maxrolls { get; set; }

        [JsonProperty(PropertyName = "Min rolls")]
        public int Minrolls { get; set; }

        public string Name { get; set; }

        [JsonProperty(PropertyName = "Oracle type")]
        public string Oracletype { get; set; }

        public bool Repeatable { get; set; }
        public Requires Requires { get; set; }

        [JsonProperty(PropertyName = "Select table by")]
        public string Selecttableby { get; set; }

        public Semantics Semantics { get; set; }
        public string Subgroup { get; set; }
        public List<ChanceTable> Table { get; set; }
        public List<Tables> Tables { get; set; }

        [JsonProperty(PropertyName = "Use with")]
        public List<UseWith> Usewith { get; set; }
    }

    public class OracleInfo
    {
        public OracleInfo()
        {
        }

        public string[] Aliases { get; set; }
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string Displayname { get; set; }

        public List<Inherit> Inherits { get; set; }
        public string Name { get; set; }
        public List<Oracle> Oracles { get; set; }
        public Source Source { get; set; }
        public List<Subcategory> Subcategories { get; set; }
        public string[] Tags { get; set; }
    }

    public partial class Requires
    {
        [JsonProperty(PropertyName = "Derelict Type")]
        public string[] DerelictType { get; set; }

        public string[] Environment { get; set; }
        public string[] Life { get; set; }
        public string[] Location { get; set; }
        public string[] Region { get; set; }
        public string[] Scale { get; set; }

        [JsonProperty(PropertyName = "Starship Type")]
        public string[] StarshipType { get; set; }
    }

    public class Rootobject
    {
        public List<OracleInfo> OracleInfos { get; set; }
    }

    public class Semantics
    {
        public bool Capitalize { get; set; }
        public object Content { get; set; }

        [JsonProperty(PropertyName = "Part of speech")]
        public string[] Partofspeech { get; set; }
    }

    public class Subcategory
    {
        public string[] Aliases { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string Displayname { get; set; }

        public List<Inherit> Inherits { get; set; }
        public string Name { get; set; }
        public List<Oracle> Oracles { get; set; }
        public object Requires { get; set; }

        [JsonProperty(PropertyName = "Sample Names")]
        public string[] SampleNames { get; set; }

        public Source Source { get; set; }
        public string Thumbnail { get; set; }
    }

    public class Suggest
    {
        [JsonProperty(PropertyName = "Game object")]
        public GameObject Gameobject { get; set; }

        public List<Oracle> Oracles { get; set; }
    }

    public class Tables
    {
        public string[] Aliases { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string Displayname { get; set; }

        public string Name { get; set; }
        public Requires Requires { get; set; }
        public List<ChanceTable> Table { get; set; }
    }

    public class UseWith
    {
        public string Category { get; set; }
        public string Name { get; set; }
    }
}