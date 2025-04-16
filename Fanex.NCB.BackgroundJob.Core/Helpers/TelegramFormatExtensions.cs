namespace Fanex.NCB.AutoJobs.Core.Helpers;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

[ExcludeFromCodeCoverage(Justification = "Helpers")]
public static class TelegramFormatExtensions
{
    private const string BoldRegex = "\\<b>$2</b>";
    public static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(100);

    public static string ToMarkdownV2(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        // convert skype format to telegram format
        text = Regex.Replace(text, "(\\*\\*\\*)(.+?)(\\*\\*\\*)", "<b><i>$2</i></b>", RegexOptions.None, RegexTimeout); // bold & italic

        text = Regex.Replace(text, @"(\\\*\\\*)(.+?)(\\\*\\\*)", BoldRegex, RegexOptions.None, RegexTimeout); // bold
        text = Regex.Replace(text, @"(\\\*\\\*)(.+?)(\\\*\*)", BoldRegex, RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"(\\\*\\\*)(.+?[^\\])(\*\*)", BoldRegex, RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"(\\\*\*)(.+?)(\\\*\*)", BoldRegex, RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"(\\\*\*)(.+?[^\\])(\*\*)", BoldRegex, RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"(\*\*)(.+?[^\\])(\*\*)", "<b>$2</b>", RegexOptions.None, RegexTimeout);

        text = Regex.Replace(text, @"(\\\*)(.+?)(\\\*)", "\\*$2\\*", RegexOptions.None, RegexTimeout); // italic
        text = Regex.Replace(text, @"(\\\*)(.+?[^\\])(\*)", "\\*$2\\*", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"(\*)(.+?[^\\])(\*)", "<i>$2</i>", RegexOptions.None, RegexTimeout);

        text = Regex.Replace(text, @"(\\~\\~)(.+?)(\\~~)", "~\\~$2~\\~", RegexOptions.None, RegexTimeout); // strikethrough
        text = Regex.Replace(text, @"(\\~\\~)(.+?[^\\])(~~)", "\\~\\~$2\\~\\~", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"(\\~~)(.+?)(\\~~)", "~\\~$2~\\~", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"(\\~~)(.+?[^\\])(~~)", "~\\~$2\\~\\~", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"(~~)(.+?[^\\])(~~)", "<s>$2</s>", RegexOptions.None, RegexTimeout);

        // escape
        text = EscapeSpecialSyntax(text);

        // revert
        text = Regex.Replace(text, @"(\\<b>)(.+?)(</b>)", "\\*\\*$2\\*\\*", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"(\\<i>)(.+?)(</i>)", "\\*$2\\*", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, "(<b>)(.+?)(</b>)", "*$2*", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, "(<i>)(.+?)(</i>)", "_$2_", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, "(<s>)(.+?)(</s>)", "~$2~", RegexOptions.None, RegexTimeout);

        // escape
        text = Regex.Replace(text, @"(.*?)(>|[\\][>])", "$1\\>", RegexOptions.None, RegexTimeout);

        return text;
    }

    private static string EscapeExceptHyperlink(string text, string pattern, string replacement)
    {
        var regex = new Regex(@"\[\S+]\(https?://\S+\)", RegexOptions.None, RegexTimeout);
        var hyperlinks = regex.Matches(text).Cast<Match>().ToList();
        return Regex.Replace(text, pattern, delegate (Match match)
        {
            return hyperlinks.Exists(x => x.Index <= match.Index && match.Index <= x.Index + x.Length - 1)
                ? match.Value
                : replacement;
        }, RegexOptions.None, RegexTimeout);
    }

    public static string EscapeSpecialSyntax(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        text = EscapeExceptHyperlink(text, @"\[|\\\[", "\\[");
        text = EscapeExceptHyperlink(text, @"]|\\\]", "\\]");
        text = EscapeExceptHyperlink(text, @"\(|\\\(", "\\(");
        text = EscapeExceptHyperlink(text, @"\)|\\\)", "\\)");

        text = Regex.Replace(text, @"_|\\_", "\\_", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"\*|\\\*", "\\*", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"~|\\~", "\\~", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"`|\\`", "\\`", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"#|\\#", "\\#", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"\+|\\\+", "\\+", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"-|\\\-", "\\-", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"=|\\=", "\\=", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"\||\\\|", "\\|", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"\{|\\\{", "\\{", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"}|\\}", "\\}", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"\.|\\\.", "\\.", RegexOptions.None, RegexTimeout);
        text = Regex.Replace(text, @"!|\\!", "\\!", RegexOptions.None, RegexTimeout);

        return text;
    }
}