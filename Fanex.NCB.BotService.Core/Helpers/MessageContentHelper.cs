using System;
using System.Text.RegularExpressions;

namespace Fanex.NCB.BotService.Core.Helpers
{
    public static class MessageContentHelper
    {
        private static readonly TimeSpan RegexTimeOut = TimeSpan.FromSeconds(100);

        public static string RemoveOrderListMardownFormat(this string content)
        {
            var newContent = content;
            var orderlistRegex = new Regex(@"((^\s*\d+)|(\n+\s*\d+))(\)|\.)", RegexOptions.None, RegexTimeOut);
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
            var regex = new Regex(@"~~.+?~~", RegexOptions.None, RegexTimeOut);
            var matches = regex.Matches(content);
            var newContent = content;
            foreach (Match match in matches)
            {
                var newValue = $"<s>{match.Value.Replace("~~", string.Empty)}</s>";
                newContent = regex.Replace(newContent, newValue, 1);
            }
            return newContent;
        }

        public static string AddExcapeCharacterToBlockQuoteMarkdown(this string content)
        {
            var newContent = content;
            var blockQuoteRegex = new Regex(@"(^\s*|\n\s*)>", RegexOptions.None, RegexTimeOut);
            var matches = blockQuoteRegex.Matches(content);
            foreach (Match match in matches)
            {
                var newValue = match.Value.Replace(">", "\\>");
                newContent = blockQuoteRegex.Replace(newContent, newValue, 1);
            }
            return newContent;
        }

        public static string FixTeamsTextFormat(this string content)
            => content.RemoveOrderListMardownFormat()
                .FixTeamsStrikeThrough()
                .AddExcapeCharacterToBlockQuoteMarkdown()
                .Replace("\n ", "\n&nbsp;")
                .Replace("\r\n", "\n").Replace("\n", "\n\n")
                .Replace("\n\n\n\n", "\n\n&nbsp;\n\n");
    }
}
