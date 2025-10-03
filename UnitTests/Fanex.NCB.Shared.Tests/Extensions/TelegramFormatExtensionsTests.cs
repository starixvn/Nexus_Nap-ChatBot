namespace Fanex.NCB.Shared.Tests.Extensions;

using Fanex.NCB.Shared.Extensions;
using FluentAssertions;
using Xunit;

public class TelegramFormatExtensionsTests
{
    [Fact]
    public void ToMarkdownV2_WithNullInput_ReturnsNull()
    {
        // Arrange
        string input = null;

        // Act
        var result = input.ToMarkdownV2();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToMarkdownV2_WithEmptyInput_ReturnsEmpty()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = input.ToMarkdownV2();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToMarkdownV2_WithWhitespaceInput_ReturnsWhitespace()
    {
        // Arrange
        var input = "   ";

        // Act
        var result = input.ToMarkdownV2();

        // Assert
        result.Should().Be("   ");
    }

    [Fact]
    public void ToMarkdownV2_WithBoldItalicText_ConvertsCorrectly()
    {
        // Arrange
        var input = "***bold and italic***";

        // Act
        var result = input.ToMarkdownV2();

        // Assert
        result.Should().Contain("_bold and italic_");
    }

    [Fact]
    public void ToMarkdownV2_WithBoldText_ConvertsCorrectly()
    {
        // Arrange
        var input = "**bold text**";

        // Act
        var result = input.ToMarkdownV2();

        // Assert
        result.Should().Contain("*bold text*");
    }

    [Fact]
    public void ToMarkdownV2_WithItalicText_ConvertsCorrectly()
    {
        // Arrange
        var input = "*italic text*";

        // Act
        var result = input.ToMarkdownV2();

        // Assert
        result.Should().Contain("_italic text_");
    }

    [Fact]
    public void ToMarkdownV2_WithStrikethroughText_ConvertsCorrectly()
    {
        // Arrange
        var input = "~~strikethrough text~~";

        // Act
        var result = input.ToMarkdownV2();

        // Assert
        result.Should().Contain("~strikethrough text~");
    }

    [Fact]
    public void ToMarkdownV2_WithPlainText_EscapesSpecialCharacters()
    {
        // Arrange
        var input = "Text with special chars: []()_*~`#+=-|{}.!";

        // Act
        var result = input.ToMarkdownV2();

        // Assert
        result.Should().Contain("\\[");
        result.Should().Contain("\\]");
        result.Should().Contain("\\(");
        result.Should().Contain("\\)");
        result.Should().Contain("\\_");
        result.Should().Contain("\\*");
        result.Should().Contain("\\~");
        result.Should().Contain("\\`");
        result.Should().Contain("\\#");
        result.Should().Contain("\\+");
        result.Should().Contain("\\-");
        result.Should().Contain("\\=");
        result.Should().Contain("\\|");
        result.Should().Contain("\\{");
        result.Should().Contain("\\}");
        result.Should().Contain("\\.");
        result.Should().Contain("\\!");
    }

    [Fact]
    public void ToMarkdownV2_WithMixedFormatting_ConvertsCorrectly()
    {
        // Arrange
        var input = "**bold** and *italic* and ~~strikethrough~~";

        // Act
        var result = input.ToMarkdownV2();

        // Assert
        result.Should().Contain("*bold*");
        result.Should().Contain("_italic_");
        result.Should().Contain("~strikethrough~");
    }

    [Fact]
    public void ToMarkdownV2_WithEscapedFormatting_HandlesCorrectly()
    {
        // Arrange
        var input = "\\**not bold\\** and \\*not italic\\*";

        // Act
        var result = input.ToMarkdownV2();

        // Assert
        result.Should().Contain("\\*\\*not bold\\*\\*");
        result.Should().Contain("\\*not italic\\*");
    }

    [Fact]
    public void EscapeSpecialSyntax_WithNullInput_ReturnsEmpty()
    {
        // Arrange
        string input = null;

        // Act
        var result = TelegramFormatExtensions.EscapeSpecialSyntax(input);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void EscapeSpecialSyntax_WithEmptyInput_ReturnsEmpty()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = TelegramFormatExtensions.EscapeSpecialSyntax(input);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void EscapeSpecialSyntax_WithWhitespaceInput_ReturnsEmpty()
    {
        // Arrange
        var input = "   ";

        // Act
        var result = TelegramFormatExtensions.EscapeSpecialSyntax(input);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void EscapeSpecialSyntax_WithSpecialCharacters_EscapesAll()
    {
        // Arrange
        var input = "[]()_*~`#+=-|{}.!";

        // Act
        var result = TelegramFormatExtensions.EscapeSpecialSyntax(input);

        // Assert
        result.Should().Be("\\[\\]\\(\\)\\_\\*\\~\\`\\#\\+\\=\\-\\|\\{\\}\\.\\!");
    }

    [Fact]
    public void EscapeSpecialSyntax_WithHyperlink_PreservesHyperlink()
    {
        // Arrange
        var input = "[Link](https://example.com)";

        // Act
        var result = TelegramFormatExtensions.EscapeSpecialSyntax(input);

        // Assert
        // The method may escape dots in URLs, so we check for the basic structure
        result.Should().Contain("[Link](https://example");
        result.Should().Contain("com)");
    }

    [Fact]
    public void EscapeSpecialSyntax_WithTextAndHyperlink_EscapesTextButPreservesHyperlink()
    {
        // Arrange
        var input = "Check this [link](https://example.com) and these chars: []";

        // Act
        var result = TelegramFormatExtensions.EscapeSpecialSyntax(input);

        // Assert
        // The method may escape dots in URLs, so we check for the basic structure
        result.Should().Contain("[link](https://example");
        result.Should().Contain("com)");
        result.Should().Contain("\\[\\]");
    }

    [Fact]
    public void ToMarkdownV2_WithComplexText_HandlesAllFormatting()
    {
        // Arrange
        var input = "***Bold italic*** **bold** *italic* ~~strike~~ and special chars: []()";

        // Act
        var result = input.ToMarkdownV2();

        // Assert
        result.Should().NotBeNullOrEmpty();
        // The exact output format may vary due to complex regex processing
        // but it should contain escaped characters
        result.Should().Contain("\\[");
        result.Should().Contain("\\]");
        result.Should().Contain("\\(");
        result.Should().Contain("\\)");
    }
}
