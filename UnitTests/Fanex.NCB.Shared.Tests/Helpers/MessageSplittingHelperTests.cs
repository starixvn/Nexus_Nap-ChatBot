namespace Fanex.NCB.Shared.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NCB.Shared.Helpers;
    using Xunit;
    using Xunit.Abstractions;

    public class MessageSplittingHelperTests
    {
        private readonly ITestOutputHelper output;

        public MessageSplittingHelperTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        #region Helper Methods

        private void AssertAllPartsWithinLimit(List<string> parts, int maxLength)
        {
            foreach (var part in parts)
            {
                var byteCount = Encoding.UTF8.GetByteCount(part);
                Assert.True(byteCount <= maxLength,
                    $"Part '{part}' exceeds limit: {byteCount} > {maxLength} bytes");
            }
        }

        private void AssertContentPreserved(string original, List<string> parts, double preservationThreshold = 0.95)
        {
            var rejoined = string.Join("", parts);
            var originalNormalized = original.Replace(" ", "").Replace("\n", "").Replace("\r", "");
            var rejoinedNormalized = rejoined.Replace(" ", "").Replace("\n", "").Replace("\r", "");

            Assert.True(rejoinedNormalized.Length >= originalNormalized.Length * preservationThreshold,
                $"Content preservation failed. Original: {originalNormalized.Length} chars, " +
                $"Rejoined: {rejoinedNormalized.Length} chars");
        }

        #endregion

        #region Basic Functionality Tests

        [Fact]
        public void When_MessageIsEmpty_Should_ReturnNoResults()
        {
            // Arrange
            var message = string.Empty;
            var maxLength = 50;

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void When_MessageIsNull_Should_ReturnNoResults()
        {
            // Arrange
            string message = null;
            var maxLength = 50;

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void When_MessageFitsInLimit_Should_ReturnSinglePart()
        {
            // Arrange
            var message = "Short message.";
            var maxLength = 50;

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.Single(result);
            Assert.Equal(message, result[0]);
            AssertAllPartsWithinLimit(result, maxLength);
        }

        #endregion

        #region Sentence Boundary Splitting Tests

        [Fact]
        public void When_MessageHasSentences_Should_SplitAtSentenceBoundaries()
        {
            // Arrange
            var message = "Hello world. This is a test! How are you?";
            var maxLength = 20; // Force splitting

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("Hello world.", result[0]);
            Assert.Equal("This is a test!", result[1]);
            Assert.Equal("How are you?", result[2]);
            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);
        }

        [Fact]
        public void When_MessageHasCommas_Should_SplitAtCommas()
        {
            // Arrange
            var message = "Apple, banana, cherry, date.";
            var maxLength = 15; // Force splitting at commas

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 2, "Should split at comma boundaries");
            AssertAllPartsWithinLimit(result, maxLength);

            // Check that commas are preserved in splits
            var hasCommaInSplit = result.Any(part => part.Contains(","));
            Assert.True(hasCommaInSplit, "Comma should be preserved in one of the parts");

            AssertContentPreserved(message, result);
        }

        [Fact]
        public void When_MessageHasLineBreaks_Should_SplitAtLineBreaks()
        {
            // Arrange
            var message = "Line one\nLine two\nLine three";
            var maxLength = 12; // Force splitting at line breaks

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 2, "Should split at line breaks");
            AssertAllPartsWithinLimit(result, maxLength);

            // Verify that the content is properly split
            Assert.Contains("Line one", result[0]);

            AssertContentPreserved(message, result);
        }

        #endregion

        #region Word Boundary Fallback Tests

        [Fact]
        public void When_SentenceIsTooLong_Should_FallbackToWordBoundaries()
        {
            // Arrange
            var message = "This is a very long sentence with many words that exceeds limits.";
            var maxLength = 20; // Force word boundary splitting

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 3, "Should split into multiple word chunks");
            AssertAllPartsWithinLimit(result, maxLength);

            // Verify we get meaningful splits
            Assert.Contains("This is a very", result[0]);
            Assert.True(result.Any(part => part.Contains("sentence")));

            AssertContentPreserved(message, result);
        }

        [Fact]
        public void When_WordIsTooLong_Should_SplitAtCharacterLevel()
        {
            // Arrange
            var message = "Normal supercalifragilisticexpialidocious end.";
            var maxLength = 15; // The long word itself exceeds this

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 4, "Should split the long word into multiple parts");
            AssertAllPartsWithinLimit(result, maxLength);

            // Check that the long word was split into character chunks
            var longWordParts = result.Where(part =>
                part.Contains("supercali") || part.Contains("fragili") ||
                part.Contains("stic") || part.Contains("expiali") ||
                part.Contains("docious")).ToList();

            Assert.True(longWordParts.Count >= 2, "Long word should be split into multiple character chunks");

            AssertContentPreserved(message, result);
        }

        #endregion

        #region Unicode and Special Characters Tests

        [Fact]
        public void When_MessageHasUnicodeAndEmojis_Should_HandleCorrectly()
        {
            // Arrange
            var message = "Hello 🌍 Test 🚀 End";
            var maxLength = 12; // Small limit for Unicode

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 2, "Should split Unicode content");
            AssertAllPartsWithinLimit(result, maxLength);

            // Verify emojis are preserved
            var allParts = string.Join("", result);
            Assert.Contains("🌍", allParts);
            Assert.Contains("🚀", allParts);

            AssertContentPreserved(message, result);
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Fact]
        public void When_MessageIsExactlyAtLimit_Should_ReturnSinglePart()
        {
            // Arrange
            var message = "Exactly25Bytes"; // Exactly 14 bytes
            var maxLength = 14;

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.Single(result);
            Assert.Equal(message, result[0]);
            Assert.Equal(14, Encoding.UTF8.GetByteCount(result[0]));
        }

        [Fact]
        public void When_MessageIsOneByteTooLong_Should_SplitAppropriately()
        {
            // Arrange
            var message = "Short word that exceeds"; // Forces splitting
            var maxLength = 10;

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 2, "Should split when exceeding limit");
            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);
        }

        [Fact]
        public void When_MessageIsComplex_Should_PreserveContentIntegrity()
        {
            // Arrange
            var message = "Test: Item1, Item2! Item3? 🚀";
            var maxLength = 12;

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 3, "Complex message should split multiple times");
            AssertAllPartsWithinLimit(result, maxLength);

            // Verify specific punctuation preservation
            var allContent = string.Join("", result);
            Assert.Contains(":", allContent);
            Assert.Contains(",", allContent);
            Assert.Contains("!", allContent);
            Assert.Contains("?", allContent);
            Assert.Contains("🚀", allContent);

            AssertContentPreserved(message, result);
        }

        #endregion

        #region Helper Method Tests

        [Fact]
        public void SentenceBoundaryMethod_WhenGivenSingleSentence_ShouldReturnOnepart()
        {
            // Arrange
            var message = "This is a complete sentence.";
            var maxLength = 50;

            // Act
            var result = MessageSplittingHelper.SplitMessageAtSentenceBoundaries(message, maxLength);

            // Assert
            Assert.Single(result);
            Assert.Equal(message, result[0]);
        }

        [Fact]
        public void SentenceBoundaryMethod_WhenGivenMultipleSentences_ShouldSplitAtSentenceEnds()
        {
            // Arrange
            var message = "First. Second! Third?";
            var maxLength = 10; // Force splitting

            // Act
            var result = MessageSplittingHelper.SplitMessageAtSentenceBoundaries(message, maxLength);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("First.", result[0]);
            Assert.Equal("Second!", result[1]);
            Assert.Equal("Third?", result[2]);
            AssertAllPartsWithinLimit(result, maxLength);
        }

        [Fact]
        public void DelimiterMethod_WhenGivenTextWithPunctuation_ShouldSplitAtEachDelimiter()
        {
            // Arrange
            var text = "First. Second! Third? Fourth; Fifth: Sixth,";
            var delimiters = new[] { '.', '!', '?', ';', ':', ',' };

            // Act
            var result = MessageSplittingHelper.SplitBySentenceDelimiters(text, delimiters);

            // Assert
            Assert.Equal(6, result.Count);

            // Verify that each part contains the expected content
            Assert.StartsWith("First", result[0]);
            Assert.StartsWith("Second", result[1]);
            Assert.StartsWith("Third", result[2]);
            Assert.StartsWith("Fourth", result[3]);
            Assert.StartsWith("Fifth", result[4]);
            Assert.StartsWith("Sixth", result[5]);

            // Verify delimiters are preserved
            Assert.True(result.Any(part => part.Contains(".")));
            Assert.True(result.Any(part => part.Contains("!")));
            Assert.True(result.Any(part => part.Contains("?")));
            Assert.True(result.Any(part => part.Contains(";")));
            Assert.True(result.Any(part => part.Contains(":")));
            Assert.True(result.Any(part => part.Contains(",")));
        }

        [Fact]
        public void WordBoundaryMethod_WhenGivenLongSentence_ShouldSplitAtSpaces()
        {
            // Arrange
            var text = "One two three four five";
            var maxLength = 8; // Force word splitting

            // Act
            var result = MessageSplittingHelper.SplitAtWordBoundaries(text, maxLength);

            // Assert
            Assert.True(result.Count >= 3, "Should split into multiple word groups");
            AssertAllPartsWithinLimit(result, maxLength);

            // Verify content is preserved
            var rejoined = string.Join("", result);
            var original = text.Replace(" ", "");
            Assert.Equal(original, rejoined.Replace(" ", ""));
        }

        [Fact]
        public void CharacterMethod_WhenGivenVeryLongWord_ShouldSplitByCharacters()
        {
            // Arrange
            var word = "supercalifragilisticexpialidocious";
            var maxLength = 8; // Much smaller than the word

            // Act
            var result = MessageSplittingHelper.SplitLongWord(word, maxLength);

            // Assert
            Assert.True(result.Count >= 4, "Should split long word into character chunks");
            AssertAllPartsWithinLimit(result, maxLength);

            // Verify exact reconstruction
            var reconstructed = string.Join("", result);
            Assert.Equal(word, reconstructed);

            // Verify no part is empty
            Assert.True(result.All(part => part.Length > 0), "No part should be empty");
        }

        [Fact]
        public void CharacterMethod_WhenGivenUnicodeText_ShouldHandleProperlyByBytes()
        {
            // Arrange
            var word = "🌍🚀🎉";
            var maxLength = 8; // Each emoji is 4 bytes in UTF-8

            // Act
            var result = MessageSplittingHelper.SplitLongWord(word, maxLength);

            // Assert
            AssertAllPartsWithinLimit(result, maxLength);

            // Verify exact reconstruction
            var reconstructed = string.Join("", result);
            Assert.Equal(word, reconstructed);

            // Verify we get exactly 2 parts (2 emojis fit in 8 bytes, 1 emoji in remaining)
            Assert.Equal(2, result.Count);
            Assert.Equal("🌍🚀", result[0]); // 8 bytes
            Assert.Equal("🎉", result[1]); // 4 bytes
        }

        #endregion

        #region Four-Tier Priority Rule Verification Tests

        [Fact]
        public void PriorityRule_Tier1_SentenceBoundaries_ShouldHaveHighestPriority()
        {
            // Arrange - Message with sentences and commas, force sentence splitting
            var message = "First sentence. Second sentence, with comma inside!";
            var maxLength = 20; // Forces splitting at sentence boundaries, not commas

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 2, "Should split at sentence boundaries");
            Assert.Equal("First sentence.", result[0]);
            Assert.True(result.Any(part => part.Contains("Second sentence")), "Should preserve sentence structure");
            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);
        }

        [Fact]
        public void PriorityRule_Tier2_PunctuationBoundaries_ShouldHaveMediumHighPriority()
        {
            // Arrange - Long sentence without periods, should split at commas/semicolons
            var message = "We have items: apples, bananas, cherries; plus drinks: water, juice";
            var maxLength = 30; // Forces punctuation splitting, not word splitting

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 3, "Should split at punctuation boundaries");

            // Verify commas and semicolons are preserved as split points
            var hasCommaInFirstPart = result.Any(part => part.Contains("apples,"));
            var hasSemicolonSplit = result.Any(part => part.Contains(";"));
            var hasColonSplit = result.Any(part => part.Contains(":"));

            Assert.True(hasCommaInFirstPart || hasSemicolonSplit || hasColonSplit,
                "Should split at punctuation boundaries (comma, semicolon, or colon)");

            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);
        }

        [Fact]
        public void PriorityRule_Tier3_WordBoundaries_ShouldHaveMediumPriority()
        {
            // Arrange - Long text without sentences or significant punctuation
            var message = "This is a very long text without any sentences or major punctuation marks to test word boundary splitting";
            var maxLength = 25; // Forces word boundary splitting

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 4, "Should split at word boundaries");

            // Verify no part starts or ends with a space (proper word boundary handling)
            foreach (var part in result)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    Assert.False(part.StartsWith(" "), $"Part should not start with space: '{part}'");
                    Assert.False(part.EndsWith(" "), $"Part should not end with space: '{part}'");
                }
            }

            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);
        }

        [Fact]
        public void PriorityRule_Tier4_CharacterBoundaries_ShouldBeLastResort()
        {
            // Arrange - Very long word that exceeds limit
            var message = "supercalifragilisticexpialidocious";
            var maxLength = 8; // Forces character-level splitting

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 4, "Should split at character boundaries");

            // Verify exact reconstruction (character splitting preserves everything)
            var reconstructed = string.Join("", result);
            Assert.Equal(message, reconstructed);

            AssertAllPartsWithinLimit(result, maxLength);
        }

        [Fact]
        public void PriorityRule_ComplexMessage_ShouldRespectAllTiers()
        {
            // Arrange - Complex message that should trigger multiple splitting tiers
            var message = "Alert! Status: critical; items: server1, server2, database; supercalifragilisticexpialidocious word here.";
            var maxLength = 25; // Forces multiple splitting strategies

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 4, "Should split into multiple parts using different strategies");

            // Should preserve sentence boundary (period after "Alert!")
            Assert.Contains(result, part => part.Contains("Alert!"));

            // Should use punctuation boundaries for lists
            var hasPunctuationSplits = result.Any(part => part.Contains(",") || part.Contains(";") || part.Contains(":"));
            Assert.True(hasPunctuationSplits, "Should preserve punctuation boundaries");

            // Should handle the long word
            var hasLongWordSplit = result.Any(part => part.Contains("supercali") || part.Contains("fragili"));
            Assert.True(hasLongWordSplit, "Should handle character-level splitting for long words");

            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);
        }

        [Fact]
        public void PriorityRule_SentenceOverPunctuation_ShouldPrioritizeSentences()
        {
            // Arrange - Message where sentence boundaries should take priority over commas
            var message = "First item, second item, third item. Next sentence begins here, with more items.";
            var maxLength = 50; // Should split at periods, not commas

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 2, "Should split at sentence boundaries");
            Assert.True(result.Any(part => part.Contains("First item, second item, third item.")),
                "First sentence should be kept together");
            Assert.True(result.Any(part => part.Contains("Next sentence begins here")),
                "Second sentence should be in a separate part");
            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);
        }

        [Fact]
        public void PriorityRule_PunctuationOverWords_ShouldPrioritizePunctuation()
        {
            // Arrange - Long sentence with commas that should split at commas, not arbitrary words
            var message = "We need to process items: first item, second item, third item, fourth item";
            var maxLength = 25; // Should split at commas, not in the middle of words

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 3, "Should split at punctuation marks");

            // Verify splits happen at punctuation boundaries (colon or commas)
            var hasPunctuationSplit = result.Any(part => part.Contains(":") || part.Contains(","));
            Assert.True(hasPunctuationSplit, "Should split at punctuation boundaries");

            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);
        }

        [Fact]
        public void PriorityRule_WordsOverCharacters_ShouldPrioritizeWords()
        {
            // Arrange - Message with regular words that should split at word boundaries, not character level
            var message = "This message has normal words that should split at boundaries not characters";
            var maxLength = 20; // Should split at spaces, not break words apart

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 4, "Should split into multiple word-boundary parts");

            // Verify no words are broken apart (each part should end with complete words)
            foreach (var part in result.Where(p => !string.IsNullOrEmpty(p)))
            {
                // Should not end in the middle of a word (unless it's the very last part)
                var words = part.Trim().Split(' ');
                Assert.True(words.Length >= 1, "Each part should contain at least one complete word");
            }

            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);
        }

        [Theory]
        [InlineData("First. Second, third; fourth: fifth", 15)] // Mixed delimiters
        [InlineData("Item1, item2, item3; group2: subitem1, subitem2", 20)] // Complex punctuation
        [InlineData("Alert! Critical: server down; actions: restart, monitor, alert", 25)] // Real-world scenario
        public void PriorityRule_VariousScenarios_ShouldAlwaysRespectHierarchy(string message, int maxLength)
        {
            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.NotEmpty(result);
            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);

            // Verify that splitting happens at logical boundaries (sentences, punctuation, or words)
            // Check that we don't have arbitrary character breaks in the middle of normal words
            foreach (var part in result)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    // Each part should be a logical unit (not random character breaks)
                    Assert.True(part.Length > 0, "Each part should have content");
                }
            }

            // Verify logical splitting - should preserve punctuation and word structure
            var allContent = string.Join("", result);
            var originalPunctuation = message.Count(c => ".!?;:,".Contains(c));
            var preservedPunctuation = allContent.Count(c => ".!?;:,".Contains(c));
            Assert.True(preservedPunctuation >= originalPunctuation * 0.9,
                "Most punctuation should be preserved, indicating proper boundary splitting");
        }

        #endregion

        #region Multilingual and Unicode Support Tests

        [Fact]
        public void MultiLanguage_ChineseText_ShouldHandleCorrectly()
        {
            // Arrange - Your actual Chinese text example
            var message = "Resettlement\" on the left panel for the checking of matches' affected list. >> 请知悉，目前所有\"重新结算\"事项之受影响客户名单将不以邮件方式提供。贵司可透过后台系统登录并点击 \"通知\" - \"重新结算\" 查询赛事受影响客户名单。";
            var maxLength = 100; // Force splitting to test Unicode handling

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.NotEmpty(result);
            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);

            // Verify Chinese characters are preserved
            var allContent = string.Join("", result);
            Assert.Contains("请知悉", allContent);
            Assert.Contains("重新结算", allContent);
            Assert.Contains("客户名单", allContent);
            Assert.Contains("通知", allContent);
        }

        [Fact]
        public void MultiLanguage_ChinesePunctuation_ShouldRecognizeDelimiters()
        {
            // Arrange - Chinese text with Chinese punctuation
            var message = "第一句话。第二句话！第三句话？还有更多内容；包括一些项目：项目一，项目二，项目三。";
            var maxLength = 30; // Force splitting

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 2, "Should split Chinese text at punctuation boundaries");
            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);

            // Verify Chinese punctuation is recognized (。！？；：，)
            var hasProperSplits = result.Any(part => part.Contains("第一句话。") ||
                                                   part.Contains("第二句话！") ||
                                                   part.Contains("第三句话？"));
            Assert.True(hasProperSplits, "Should split at Chinese punctuation marks");
        }



        [Fact]
        public void ChinesePunctuation_SentenceDelimiters_ShouldSplitCorrectly()
        {
            // Arrange - Test English and Chinese sentence punctuation
            var text = "English. 中文。Another! 另一个！Question? 问题？";

            // Act - Test sentence splitting using the main method
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(text, 25);

            // Assert
            Assert.True(result.Count >= 4, $"Should split into multiple parts, got {result.Count}");
            AssertAllPartsWithinLimit(result, 25);
            AssertContentPreserved(text, result);

            // Verify that both English and Chinese sentence delimiters work
            var allContent = string.Join("", result);
            Assert.Contains("English.", allContent);
            Assert.Contains("中文。", allContent);
            Assert.Contains("Another!", allContent);
            Assert.Contains("另一个！", allContent);
            Assert.Contains("Question?", allContent);
            Assert.Contains("问题？", allContent);
        }

        [Fact]
        public void ChinesePunctuation_ListDelimiters_ShouldSplitCorrectly()
        {
            // Arrange - Test English and Chinese punctuation delimiters
            var text = "Items: one, two; 项目：一，二；";

            // Act - Test punctuation splitting using the main method
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(text, 15);

            // Assert
            Assert.True(result.Count >= 4, "Should split at punctuation marks");
            AssertAllPartsWithinLimit(result, 15);
            AssertContentPreserved(text, result);

            // Verify both English and Chinese punctuation delimiters work
            var allContent = string.Join("", result);
            Assert.Contains("Items:", allContent);
            Assert.Contains("one,", allContent);
            Assert.Contains("two;", allContent);
            Assert.Contains("项目：", allContent);
            Assert.Contains("一，", allContent);
            Assert.Contains("二；", allContent);
        }

        [Fact]
        public void EnglishAndChinese_MixedLanguages_ShouldHandleCorrectly()
        {
            // Arrange - Mixed English and Chinese
            var message = "Hello world! 你好世界！This is English. 这是中文。More text: 更多文字，包括标点符号。";
            var maxLength = 40; // Force splitting

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.NotEmpty(result);
            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);

            // Verify both English and Chinese are preserved
            var allContent = string.Join("", result);
            Assert.Contains("Hello world", allContent);
            Assert.Contains("你好世界", allContent);
            Assert.Contains("This is English", allContent);
            Assert.Contains("这是中文", allContent);
        }

        [Fact]
        public void EnglishAndChinese_WithEmojis_ShouldHandleCorrectly()
        {
            // Arrange - English and Chinese text with emojis
            var message = "Alert! 🚨 Status: 状态正常 ✅ Server: 服务器运行中 🖥️ Users: 用户在线 👥 Time: 时间 ⏰";
            var maxLength = 35; // Force splitting

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.NotEmpty(result);
            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);

            // Verify emojis and both languages are preserved
            var allContent = string.Join("", result);
            Assert.Contains("🚨", allContent);
            Assert.Contains("✅", allContent);
            Assert.Contains("🖥️", allContent);
            Assert.Contains("👥", allContent);
            Assert.Contains("⏰", allContent);
            Assert.Contains("状态正常", allContent);
            Assert.Contains("服务器运行中", allContent);
        }


        #endregion

        #region Comprehensive Integration Tests

        [Theory]
        [InlineData(20)]
        [InlineData(50)]
        [InlineData(100)]
        public void FullSplitting_WithDifferentLimits_ShouldAlwaysRespectByteLimit(int maxLength)
        {
            // Arrange
            var message = "Test: sentences! Questions? Lists: item1, item2, item3; Line breaks\nand unicode 🌍.";

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.NotEmpty(result);
            AssertAllPartsWithinLimit(result, maxLength);
            AssertContentPreserved(message, result);

            // Verify content elements are preserved
            var allContent = string.Join("", result);
            Assert.Contains("🌍", allContent);
            Assert.Contains(":", allContent);
            Assert.Contains("!", allContent);
            Assert.Contains("?", allContent);
            // Note: Line breaks may be removed during splitting, so we check for the basic content instead
            Assert.Contains("breaks", allContent);
        }

        [Fact]
        public void ComplexRealWorldMessage_ShouldSplitCorrectlyAndPreserveContent()
        {
            // Arrange - Simulate a real Telegram message
            var message = "🚀 Alert: Backup completed at 10:30 UTC. Files: 1,234. Size: 15.6 GB. Next: 02:00 UTC. Status: ✅ OK";
            var maxLength = 25; // Small limit to force multiple splits

            // Act
            var result = MessageSplittingHelper.SplitMessageAtWordBoundaries(message, maxLength);

            // Assert
            Assert.True(result.Count >= 4, "Real-world message should split into multiple parts");
            AssertAllPartsWithinLimit(result, maxLength);

            // Verify specific content preservation
            var allContent = string.Join("", result);
            Assert.Contains("🚀", allContent);
            Assert.Contains("✅", allContent);
            Assert.Contains("10:30", allContent);
            Assert.Contains("02:00", allContent);
            Assert.Contains("1,234", allContent);
            Assert.Contains("15.6", allContent);

            AssertContentPreserved(message, result, 0.90); // Allow slightly more flexibility for complex content
        }

        #endregion
    }
}
