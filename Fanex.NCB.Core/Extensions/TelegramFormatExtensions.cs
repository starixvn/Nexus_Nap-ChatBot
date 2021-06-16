namespace Fanex.NCB.Core.Extensions
{
    using System.Text.RegularExpressions;

    public static class TelegramFormatExtensions
    {
        public static string ToMarkdownV2(this string text)
        {
            // bold & italic
            text = Regex.Replace(text, "(\\*\\*\\*)(.+?)(\\*\\*\\*)", "<b>_$2_</b>");

            // bold
            text = Regex.Replace(text, "(\\*\\*)(.+?)(\\*\\*)", "<b>$2</b>");

            // italic
            text = Regex.Replace(text, "(\\*)(.+?)(\\*)", "_$2_");

            // strikethrough
            text = Regex.Replace(text, "(~~)(.+?)(~~)", "~$2~");

            // revert
            text = Regex.Replace(text, "(<b>)(.+?)(</b>)", "*$2*");

            return text;
        }

        public static string EscapedMarkdownV2(this string text)
        {
            // bold
            text = Regex.Replace(text, "(\\*)(.+?)(\\*)", "<b>$2</b>");

            // italic
            text = Regex.Replace(text, "(_)(.+?)(_)", "<i>$2</i>");

            // strikethrough
            text = Regex.Replace(text, "(~)(.+?)(~)", "<s>$2</s>");

            // escape
            text = Regex.Replace(text, @"(.*?)(_|[\\][_])", "$1\\_");
            text = Regex.Replace(text, @"(.*?)(\*|[\\][*])", "$1\\*");
            text = Regex.Replace(text, @"(.*?)(\[|[\\][\[])", "$1\\[");
            text = Regex.Replace(text, @"(.*?)(]|[\\][\]])", "$1\\]");
            text = Regex.Replace(text, @"(.*?)(\(|[\\][\(])", "$1\\(");
            text = Regex.Replace(text, @"(.*?)(\)[\\][\)])", "$1\\)");
            text = Regex.Replace(text, @"(.*?)(~|[\\][~])", "$1\\~");
            text = Regex.Replace(text, @"(.*?)(`|[\\][`])", "$1\\`");
            text = Regex.Replace(text, @"(.*?)(#|[\\][#])", "$1\\#");
            text = Regex.Replace(text, @"(.*?)(\+|[\\][\+])", "$1\\+");
            text = Regex.Replace(text, @"(.*?)(-|[\\][-])", "$1\\-");
            text = Regex.Replace(text, @"(.*?)(=|[\\][=])", "$1\\=");
            text = Regex.Replace(text, @"(.*?)(\||[\\][\|])", "$1\\|");
            text = Regex.Replace(text, @"(.*?)(\{|[\\][\{])", "$1\\{");
            text = Regex.Replace(text, @"(.*?)(}|[\\][}])", "$1\\}");
            text = Regex.Replace(text, @"(.*?)(\.|[\\][\.])", "$1\\.");
            text = Regex.Replace(text, @"(.*?)(!|[\\][!])", "$1\\!");

            // revert
            text = Regex.Replace(text, "(<b>)(.+?)(</b>)", "*$2*");
            text = Regex.Replace(text, "(<i>)(.+?)(</i>)", "_$2_");
            text = Regex.Replace(text, "(<s>)(.+?)(</s>)", "~$2~");

            // escape
            text = Regex.Replace(text, @"(.*?)(>|[\\][>])", "$1\\>");

            return text;
        }
    }
}