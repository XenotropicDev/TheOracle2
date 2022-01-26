using System.Text.RegularExpressions;

namespace TheOracle2.Utilities;

public class RulesText
{
    public RulesText(string text)
    {
        RawText = text;
    }

    public string RawText { get; }

    public override string ToString()
    {
        return RawText;
    }

    private Regex MdLinks { get => new Regex(@"\[(.*?)\]\(.*?\)"); }

    public string FakeLinks(string text)
    {
        return MdLinks.Replace(text, "__$1__");
    }

    private Regex MdUnorderedList { get => new Regex(@"^  +\* (.*)$", RegexOptions.Multiline); }

    private string FakeUnorderdList(string text)
    {
        return MdUnorderedList.Replace(text, "  • $1");
    }

    private Regex MdLevel1Heading { get => new Regex(@"^# (.*)$", RegexOptions.Multiline); }
    private Regex MdSubheadings { get => new Regex(@"^(##+) (.*)$", RegexOptions.Multiline); }
    // public EmbedBuilder ToEmbedWithFakeHeadings(string text = RawText) {
    //   EmbedBuilder output = new EmbedBuilder();
    //   if (text.)
    //   return output;
    // }

    // private Regex MdTable { get => }

    // private Regex MdTableCell { get => new Regex(@"([^\| ][^\|]*?) *\|"); }

    // regex:
}
