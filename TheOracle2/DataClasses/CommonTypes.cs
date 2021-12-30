using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle2.DataClasses
{
    public class Special
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public record Source
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Page")]
        public string Page { get; set; }

        [JsonProperty("Date", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Date { get; set; }
    }
}
