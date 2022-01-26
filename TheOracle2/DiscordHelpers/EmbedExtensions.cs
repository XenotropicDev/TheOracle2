namespace TheOracle2.DiscordHelpers
{
    public static class EmbedExtensions
    {
        /// <summary>
        /// Apply strikethrough to all text in a field.
        /// </summary>
        public static EmbedFieldBuilder Strike(this EmbedFieldBuilder field)
        {
            return field.WrapIn("~~");
        }
        /// <summary>
        /// Remove any strikethrough wrapping the Name and Value of this field.
        /// </summary>
        public static EmbedFieldBuilder Unstrike(this EmbedFieldBuilder field)
        {
            return field.UnwrapFrom("~~");
        }
        private static EmbedFieldBuilder WrapIn(this EmbedFieldBuilder field, string tag)
        {
            return field
                .WithName(field.Name.WrapIn(tag))
                .WithValue(field.Value.ToString().WrapIn(tag))
            ;
        }
        private static EmbedFieldBuilder UnwrapFrom(this EmbedFieldBuilder field, string tag)
        {
            return field
                .WithName(field.Name.UnwrapFrom(tag))
                .WithValue(field.Value.ToString().UnwrapFrom(tag))
            ;
        }
        public static EmbedAuthorBuilder ToEmbedAuthorBuilder(this EmbedAuthor author)
        {
            return new EmbedAuthorBuilder().WithName(author.Name).WithUrl(author.Url).WithIconUrl(author.IconUrl);
        }
        /// <summary>
        /// Changes a numeric field on an embed
        /// </summary>
        /// <param name="embed">the embed builder to change</param>
        /// <param name="fieldName">the name of the field to look for</param>
        /// <param name="change">the amount to change by (use negative numbers to decrease)</param>
        /// <param name="min">the lowest the value can be</param>
        /// <param name="max">the highest the value can be</param>
        /// <returns>The embed builder provided</returns>
        public static EmbedBuilder ChangeNumericField(this EmbedBuilder embed, string fieldName, int change, int min, int max)
        {
            int index = embed.Fields.FindIndex(f => f.Name == fieldName);
            if (int.TryParse(embed.Fields[index].Value.ToString(), out var value))
            {
                value += change;
                if (value < min) value = min;
                if (value > max) value = max;
                embed.Fields[index].Value = value;
            }
            return embed;
        }
    }
}
