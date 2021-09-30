namespace Fanex.NCB.Shared.Extensions
{
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class TelegramFormatExtensions
    {
        public static string ToMarkdownV2(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }


            text = Regex.Replace(text, @"(\\\*\*)(.+?[^\\])(\*\*)", "\\<b>$2</b>"); // bold
            text = Regex.Replace(text, @"(\\\*\*)(.+?)(\\\*\*)", "\\<b>$2</b>");
            text = Regex.Replace(text, @"(\\\*\\\*)(.+?[^\\])(\*\*)", "\\<b>$2</b>");
            text = Regex.Replace(text, @"(\\\*\\\*)(.+?)(\\\*\*)", "\\<b>$2</b>");
            text = Regex.Replace(text, @"(\\\*\\\*)(.+?)(\\\*\\\*)", "\\<b>$2</b>");
            text = Regex.Replace(text, @"(\*\*)(.+?)(\*\*)", "<b>$2</b>");

            text = Regex.Replace(text, @"(\\\*)(.+?[^\\])(\*)", "\\<i>$2</i>"); // italic
            text = Regex.Replace(text, @"(\\\*)(.+?)(\\\*)", "\\<i>$2</i>");
            text = Regex.Replace(text, @"(\*)(.+?)(\*)", "<i>$2</i>");

            text = Regex.Replace(text, @"(\\~~)(.+?[^\\])(~~)", "~\\~$2\\~\\~"); // strikethrough
            text = Regex.Replace(text, @"(\\~~)(.+?[\\])(~~)", "~\\~$2~\\~");
            text = Regex.Replace(text, @"(\\~\\~)(.+?[^\\])(~~)", "\\~\\~$2\\~\\~");
            text = Regex.Replace(text, @"(\\~\\~)(.+?[\\])(~~)", "~\\~$2~\\~");
            text = Regex.Replace(text, @"(~~)(.+?)(~~)", "<s>$2</s>");

            // escape
            text = EscapeSpecialSyntax(text);

            // revert
            text = Regex.Replace(text, @"(\\<b>)(.+?)(</b>)", "\\*\\*$2\\*\\*");
            text = Regex.Replace(text, @"(\\<i>)(.+?)(</i>)", "\\*$2\\*");

            pattern = new Regex("(<i>)(.*?)(</i>)");
            text = pattern.Replace(text, "_$2_");

            pattern = new Regex("(<s>)(.*?)(</s>)");
            text = pattern.Replace(text, "~$2~");

            // escape
            text = text.Replace(">", "\\>");

            return text;
        }

        private static string EscapeExceptHyperlink(string text, string pattern, string replacement)
        {
            var regex = new Regex(@"\[\S+]\(https?://\S+\)");
            var hyperlinks = regex.Matches(text).Cast<Match>().ToList();
            return Regex.Replace(text, pattern, delegate (Match match)
            {
                return hyperlinks.Any(x => x.Index <= match.Index && match.Index <= (x.Index + x.Length - 1))
                    ? match.Value : replacement;
            });
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

            text = Regex.Replace(text, @"_|\\_", "\\_");
            text = Regex.Replace(text, @"\*|\\\*", "\\*");
            text = Regex.Replace(text, @"~|\\~", "\\~");
            text = Regex.Replace(text, @"`|\\`", "\\`");
            text = Regex.Replace(text, @"#|\\#", "\\#");
            text = Regex.Replace(text, @"\+|\\\+", "\\+");
            text = Regex.Replace(text, @"-|\\\-", "\\-");
            text = Regex.Replace(text, @"=|\\=", "\\=");
            text = Regex.Replace(text, @"\||\\\|", "\\|");
            text = Regex.Replace(text, @"\{|\\\{", "\\{");
            text = Regex.Replace(text, @"}|\\}", "\\}");
            text = Regex.Replace(text, @"\.|\\\.", "\\.");
            text = Regex.Replace(text, @"!|\\!", "\\!");

            return text;
        }
    }
}