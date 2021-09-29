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
            text = EscapeSpecialSyntax(text);
            // revert
            pattern = new Regex("(<b>)(.*?)(</b>)");
            text = pattern.Replace(text, $"*$2*");

            pattern = new Regex("(<i>)(.*?)(</i>)");
            text = pattern.Replace(text, "_$2_");

            pattern = new Regex("(<s>)(.*?)(</s>)");
            text = pattern.Replace(text, "~$2~");

            // escape
            text = text.Replace(">", "\\>");

            text = Regex.Replace(text, "\\*.+?\\*", delegate (Match match)
            {
                return match.Index > 0 && text[match.Index - 1] == '\\'
                    ? Regex.Replace(match.Value, "(\\*)(.+?)(\\*)", "*\\*$2\\*\\*")
                    : match.Value;
            }); // bold
            text = Regex.Replace(text, "_.+?_", delegate (Match match)
            {
                return match.Index > 0 && text[match.Index - 1] == '\\'
                    ? Regex.Replace(match.Value, "(_)(.+?)(_)", "*$2\\*")
                    : match.Value;
            }); // italic
            text = Regex.Replace(text, "~.+?~", delegate (Match match)
            {
                return match.Index > 0 && text[match.Index - 1] == '\\'
                    ? Regex.Replace(match.Value, "(~)(.+?)(~)", "~\\~$2\\~\\~")
                    : match.Value;
            }); // strikethrough

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