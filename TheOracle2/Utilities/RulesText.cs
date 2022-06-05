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

    private static readonly Regex mdLinkRegex = new(@"\[(.*?)\]\(.*?\)");

    public string FakeLinks(string text)
    {
        return mdLinkRegex.Replace(text, "__$1__");
    }

    private static readonly Regex mdListRegex = new(@"^  +\* (.*)$", RegexOptions.Multiline); 

    private string FakeUnorderdList(string text)
    {
        return mdListRegex.Replace(text, "  • $1");
    }

    private Regex MdLevel1Heading { get => new Regex(@"^# (.*)$", RegexOptions.Multiline); }
    private Regex MdSubheadings { get => new Regex(@"^(##+) (.*)$", RegexOptions.Multiline); }

}
