using System;

namespace TheOracle2
{
    public class SlashCommandAttribute : Attribute
    {
        private string name;

        public string Name { get => name; private set => name = value.ToLower(); }

        public SlashCommandAttribute(string name)
        {
            Name = name;
        }
    }
}
