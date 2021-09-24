namespace Fanex.NCB.Core.Extensions
{
    using System.Text.RegularExpressions;

    public static class TelegramFormatExtensions
    {
        public static string ToMarkdownV2(this string text)
        {
            // bold
            var pattern = new Regex("(\\*\\*)(.*?)(\\*\\*)");
            text = pattern.Replace(text, $"<b>$2</b>");

            // italic
            pattern = new Regex("(\\*)(.*?)(\\*)");
            text = pattern.Replace(text, "_$2_");

            // strikethrough
            pattern = new Regex("(~~)(.*?)(~~)");
            text = pattern.Replace(text, "~$2~");

            // revert
            pattern = new Regex("(<b>)(.*?)(</b>)");
            text = pattern.Replace(text, $"*$2*");

            return text;
        }

        public static string EscapedMarkdownV2(this string text)
        {
            // bold
            var pattern = new Regex("(\\*)(.*?)(\\*)");
            text = pattern.Replace(text, $"<b>$2</b>");

            // italic
            pattern = new Regex("(_)(.*?)(_)");
            text = pattern.Replace(text, "<i>$2</i>");

            // strikethrough
            pattern = new Regex("(~)(.*?)(~)");
            text = pattern.Replace(text, "<s>$2</s>");

            // escape
            text = text.Replace("_", "\\_").Replace("*", "\\*")
                .Replace("[", "\\[").Replace("]", "\\]")
                .Replace("(", "\\(").Replace(")", "\\)")
                .Replace("~", "\\~").Replace("`", "\\`").Replace("#", "\\#")
                .Replace("+", "\\+").Replace("-", "\\-")
                .Replace("=", "\\=").Replace("|", "\\|")
                .Replace("{", "\\{").Replace("}", "\\}")
                .Replace(".", "\\.").Replace("!", "\\!");

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
    }
}