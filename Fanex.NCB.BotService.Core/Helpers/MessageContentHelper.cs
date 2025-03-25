using System.Text.RegularExpressions;

namespace Fanex.NCB.BotService.Core.Helpers
{
    public static class MessageContentHelper
    {
        public static string RemoveOrderListMardownFormat(this string content)
        {
            var newContent = content;
            var orderlistRegex = new Regex(@"((^\s*\d+)|(\n+\s*\d+))(\)|\.)");
            var matches = orderlistRegex.Matches(content);
            foreach (Match match in matches)
            {
                var newValue = match.Value.Replace(".", "\\.")
                    .Replace(")", "\\)");
                newContent = orderlistRegex.Replace(newContent, newValue, 1);
            }

            return newContent;
        }

        public static string FixTeamsStrikeThrough(this string content)
        {
            var regex = new Regex(@"~~.+?~~");
            var matches = regex.Matches(content);
            var newContent = content;
            foreach (Match match in matches)
            {
                var newValue = $"<s>{match.Value.Replace("~~", string.Empty)}</s>";
                newContent = regex.Replace(newContent, newValue, 1);
            }
            return newContent;
        }

        public static string FixTeamsTextFormat(this string content)
            => content.RemoveOrderListMardownFormat()
                .Replace("\n ", "\n&nbsp;")
                .FixTeamsStrikeThrough()
                .Replace("\r\n", "\n").Replace("\n", "\n\n")
                .Replace("\n\n\n\n", "\n\n&nbsp;\n\n");
    }
}
