namespace Fanex.NCB.Shared.Extensions
{
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class TelegramFormatExtensions
    {
        public static string ToMarkdownV2(this string text)
        {
            text = Regex.Replace(text, "(\\*)(.+?)(\\*)", "<i>$2</i>"); // italic
            var pattern = new Regex("(\\*\\*)(.*?)(\\*\\*)");
            text = pattern.Replace(text, $"<b>$2</b>");
            pattern = new Regex("(\\*)(.*?)(\\*)");
            text = pattern.Replace(text, "_$2_");
            pattern = new Regex("(~~)(.*?)(~~)");
            text = pattern.Replace(text, "~$2~");
            pattern = new Regex("(<b>)(.*?)(</b>)");
            text = pattern.Replace(text, $"*$2*");
            var pattern = new Regex("(\\*)(.*?)(\\*)");
            text = pattern.Replace(text, $"<b>$2</b>");
            pattern = new Regex("(_)(.*?)(_)");
            text = pattern.Replace(text, "<i>$2</i>");
            pattern = new Regex("(~)(.*?)(~)");
            text = pattern.Replace(text, "<s>$2</s>");

            // escape
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

            text = Regex.Replace(text, @"(\\<b>)(.+?)(</b>)", "\\<b>$2\\</b>");
            text = Regex.Replace(text, @"(\\<i>)(.+?)(</i>)", "\\<i>$2\\</i>");
            text = Regex.Replace(text, @"(\\<s>)(.+?)(</s>)", "\\<s>$2\\</s>");

            // revert
            pattern = new Regex("(<b>)(.*?)(</b>)");
            text = pattern.Replace(text, $"*$2*");

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
    }
}