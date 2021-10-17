using System.Collections.Generic;

namespace TheOracle2
{
    public class MovesInfo
    {
        public string Name { get; set; }
        public Source Source { get; set; }
        public List<Move> Moves { get; set; }
    }

    public class Source
    {
        public string Name { get; set; }
        public string Date { get; set; }
    }

    public class Move
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public List<Trigger> Triggers { get; set; }
        public bool Oracle { get; set; }
        public string Text { get; set; }
        public bool ProgressMove { get; set; }
    }

    public class Trigger
    {
        public string Text { get; set; }
        public string Details { get; set; }
        public StatOptions StatOptions { get; set; }
    }

    public class StatOptions
    {
        public string[] Stats { get; set; }
        public string Method { get; set; }
    }
}