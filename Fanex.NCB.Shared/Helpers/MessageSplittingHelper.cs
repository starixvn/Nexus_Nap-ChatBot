namespace Fanex.NCB.Shared.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

[ExcludeFromCodeCoverage(Justification = "Helpers")]
public static class MessageSplittingHelper
{
    public static List<string> SplitMessageAtWordBoundaries(string message, int maxLength)
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(message))
        {
            return result;
        }

        if (Encoding.UTF8.GetByteCount(message) <= maxLength)
        {
            result.Add(message);
            return result;
        }

        return SplitMessageAtSentenceBoundaries(message, maxLength);
    }

    public static List<string> SplitMessageAtSentenceBoundaries(string message, int maxLength)
    {
        var result = new List<string>();
        var sentenceDelimiters = new[] { '.', '!', '?', '\n', '\r' };

        var sentences = SplitBySentenceDelimiters(message, sentenceDelimiters);
        var currentMessage = "";

        foreach (var sentence in sentences)
        {
            var testMessage = BuildCombinedMessage(currentMessage, sentence);
            var testMessageByteCount = Encoding.UTF8.GetByteCount(testMessage);

            if (testMessageByteCount <= maxLength)
            {
                currentMessage = testMessage;
                continue;
            }

            AddCurrentSentenceIfNotEmpty(result, currentMessage);
            currentMessage = ProcessSentenceThatExceedsLimit(result, sentence, maxLength);
        }

        AddCurrentSentenceIfNotEmpty(result, currentMessage);
        return result;
    }

    public static List<string> SplitAtPunctuationBoundaries(string text, int maxLength)
    {
        var result = new List<string>();
        var punctuationDelimiters = new[] { ';', ':', ',' };

        var segments = SplitBySentenceDelimiters(text, punctuationDelimiters);
        var currentMessage = "";

        foreach (var segment in segments)
        {
            var testMessage = BuildCombinedMessage(currentMessage, segment);
            var testMessageByteCount = Encoding.UTF8.GetByteCount(testMessage);

            if (testMessageByteCount <= maxLength)
            {
                currentMessage = testMessage;
                continue;
            }

            AddCurrentSentenceIfNotEmpty(result, currentMessage);
            currentMessage = ProcessSegmentThatExceedsLimit(result, segment, maxLength);
        }

        AddCurrentSentenceIfNotEmpty(result, currentMessage);
        return result;
    }

    public static List<string> SplitBySentenceDelimiters(string text, char[] delimiters)
    {
        var result = new List<string>();
        var currentSentence = new StringBuilder();

        int i = 0;
        while (i < text.Length)
        {
            currentSentence.Append(text[i]);

            if (delimiters.Contains(text[i]))
            {
                if (text[i] == '\n' || text[i] == '\r')
                {
                    result.Add(currentSentence.ToString());
                    currentSentence.Clear();
                    i++;
                    continue;
                }

                if (i < text.Length - 1 && char.IsWhiteSpace(text[i + 1]))
                {
                    currentSentence.Append(text[i + 1]);
                    result.Add(currentSentence.ToString());
                    currentSentence.Clear();
                    const int skipDelimiterAndWhitespace = 2;
                    i += skipDelimiterAndWhitespace;
                    continue;
                }

                if (i == text.Length - 1)
                {
                    result.Add(currentSentence.ToString());
                    currentSentence.Clear();
                }
            }

            i++;
        }

        if (currentSentence.Length > 0)
        {
            result.Add(currentSentence.ToString());
        }

        return result;
    }

    public static List<string> SplitAtWordBoundaries(string text, int maxLength)
    {
        var result = new List<string>();
        var words = text.Split(new[] { ' ', '\t' }, StringSplitOptions.None);
        var currentMessage = "";

        foreach (var word in words)
        {
            var testMessage = BuildTestMessage(currentMessage, word);
            var testMessageByteCount = Encoding.UTF8.GetByteCount(testMessage);

            if (testMessageByteCount <= maxLength)
            {
                currentMessage = testMessage;
                continue;
            }

            AddCurrentMessageIfNotEmpty(result, currentMessage);
            currentMessage = ProcessWordThatExceedsLimit(result, word, maxLength);
        }

        AddCurrentMessageIfNotEmpty(result, currentMessage);
        return result;
    }

    public static List<string> SplitLongWord(string word, int maxLength)
    {
        var result = new List<string>();

        if (string.IsNullOrEmpty(word))
        {
            return result;
        }

        var currentPart = "";

        foreach (char c in word)
        {
            var testPart = currentPart + c;

            if (Encoding.UTF8.GetByteCount(testPart) <= maxLength)
            {
                currentPart = testPart;
            }
            else
            {
                if (!string.IsNullOrEmpty(currentPart))
                {
                    result.Add(currentPart);
                }
                currentPart = c.ToString();
            }
        }

        if (!string.IsNullOrEmpty(currentPart))
        {
            result.Add(currentPart);
        }

        return result;
    }

    #region Private Helper Methods

    private static string BuildCombinedMessage(string currentMessage, string sentence)
    {
        return string.IsNullOrEmpty(currentMessage) ? sentence : currentMessage + sentence;
    }

    private static void AddCurrentSentenceIfNotEmpty(List<string> result, string currentMessage)
    {
        if (!string.IsNullOrEmpty(currentMessage))
        {
            result.Add(currentMessage.TrimEnd());
        }
    }

    private static string ProcessSentenceThatExceedsLimit(List<string> result, string sentence, int maxLength)
    {
        var sentenceByteCount = Encoding.UTF8.GetByteCount(sentence);

        if (sentenceByteCount > maxLength)
        {
            result.AddRange(SplitAtPunctuationBoundaries(sentence, maxLength));
            return "";
        }

        return sentence;
    }

    private static string ProcessSegmentThatExceedsLimit(List<string> result, string segment, int maxLength)
    {
        var segmentByteCount = Encoding.UTF8.GetByteCount(segment);

        if (segmentByteCount > maxLength)
        {
            result.AddRange(SplitAtWordBoundaries(segment, maxLength));
            return "";
        }

        return segment;
    }

    private static string BuildTestMessage(string currentMessage, string word)
    {
        return string.IsNullOrEmpty(currentMessage) ? word : $"{currentMessage} {word}";
    }

    private static void AddCurrentMessageIfNotEmpty(List<string> result, string currentMessage)
    {
        if (!string.IsNullOrEmpty(currentMessage))
        {
            result.Add(currentMessage);
        }
    }

    private static string ProcessWordThatExceedsLimit(List<string> result, string word, int maxLength)
    {
        var wordByteCount = Encoding.UTF8.GetByteCount(word);

        if (wordByteCount > maxLength)
        {
            result.AddRange(SplitLongWord(word, maxLength));
            return "";
        }

        return word;
    }

    #endregion Private Helper Methods
}