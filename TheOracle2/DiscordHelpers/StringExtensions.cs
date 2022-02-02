using System.Text.RegularExpressions;
namespace TheOracle2.DiscordHelpers;

public static class StringExtensions
{
    public static string WrapIn(this string textToWrap, string tag)
    {
        return tag + textToWrap + tag;
    }
    public static string UnwrapFrom(this string textToUnwrap, string tag)
    {
        string result = Regex.Replace(textToUnwrap, $"^{Regex.Escape(tag)}(.*){Regex.Escape(tag)}$", "$1");
        return result;
    }
}
