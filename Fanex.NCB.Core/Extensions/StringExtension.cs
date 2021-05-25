namespace Fanex.NCB.Core.Extensions
{
    using System.Text.RegularExpressions;

    public static class StringExtension
    {
        public static string ToTelegramFormat(this string text)
        {
            const string TempFormat = "!@Markdown$V2@!";

            // bold
            var pattern = new Regex("(\\*\\*)(.*?)(\\*\\*)");
            text = pattern.Replace(text, $"{TempFormat}$2{TempFormat}");

            // italic
            pattern = new Regex("(\\*)(.*?)(\\*)");
            text = pattern.Replace(text, "_$2_");

            // strikethrough
            pattern = new Regex("(~~)(.*?)(~~)");
            text = pattern.Replace(text, "~$2~");

            // revert
            text = text.Replace(TempFormat, "*");

            return text;
        }
    }
}