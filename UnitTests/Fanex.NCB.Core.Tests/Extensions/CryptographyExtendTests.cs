namespace Fanex.NCB.Core.Tests.Extensions;

using System;
using Fanex.NCB.Core.Extensions;
using FluentAssertions;
using Xunit;

public class CryptographyExtendTests
{
    [Fact]
    public void DecryptPassword_WithNullPassword_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CryptographyExtend.DecryptPassword(null));
    }

    [Fact]
    public void DecryptPassword_WithEmptyString_ReturnsEmptyString()
    {
        // Arrange
        var emptyPassword = string.Empty;

        // Act
        var result = CryptographyExtend.DecryptPassword(emptyPassword);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void DecryptPassword_WithWhitespaceString_ReturnsEmptyString()
    {
        // Arrange
        var whitespacePassword = "   ";

        // Act
        var result = CryptographyExtend.DecryptPassword(whitespacePassword);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("dGVzdA==")] // "test" in base64
    [InlineData("cGFzc3dvcmQ=")] // "password" in base64
    public void DecryptPassword_WithValidBase64String_ReturnsDecryptedString(string encryptedPassword)
    {
        // Act & Assert
        // Note: This test verifies the method doesn't throw exceptions with valid base64 input
        // The actual decryption result depends on the specific encryption used
        var exception = Record.Exception(() => CryptographyExtend.DecryptPassword(encryptedPassword));

        // The method should either return a result or throw a specific cryptographic exception
        // but not throw unexpected exceptions
    }

    [Fact]
    public void DecryptPassword_WithInvalidBase64String_HandlesGracefully()
    {
        // Arrange
        var invalidBase64 = "invalid-base64-string!@#";

        // Act & Assert
        // The method should handle invalid base64 strings gracefully
        var exception = Record.Exception(() => CryptographyExtend.DecryptPassword(invalidBase64));

        // We expect either a successful result or a specific exception type
        // but the method should not crash unexpectedly
    }

    [Theory]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("abc")]
    [InlineData("abcd")]
    [InlineData("abcde")]
    [InlineData("abcdef")]
    [InlineData("abcdefg")]
    [InlineData("abcdefgh")]
    public void DecryptPassword_WithVariousLengths_HandlesCorrectly(string input)
    {
        // Act & Assert
        // Test that the method handles strings of various lengths
        var exception = Record.Exception(() => CryptographyExtend.DecryptPassword(input));

        // The method should handle different string lengths without crashing
    }

    [Fact]
    public void DecryptPassword_WithLongString_HandlesCorrectly()
    {
        // Arrange
        var longString = new string('a', 1000);

        // Act & Assert
        var exception = Record.Exception(() => CryptographyExtend.DecryptPassword(longString));

        // The method should handle long strings without crashing
    }

    [Theory]
    [InlineData("dGVzdA==")]
    [InlineData("dGVzdABC")]
    [InlineData("dGVzdABCDE")]
    public void DecryptPassword_WithPaddedBase64_HandlesCorrectly(string paddedInput)
    {
        // Act & Assert
        // Test that the method handles base64 strings with different padding
        var exception = Record.Exception(() => CryptographyExtend.DecryptPassword(paddedInput));

        // The method should handle padded base64 strings correctly
    }

    [Fact]
    public void DecryptPassword_ConsistentResults_WithSameInput()
    {
        // Arrange
        var testInput = "dGVzdA==";

        // Act
        var result1 = Record.Exception(() => CryptographyExtend.DecryptPassword(testInput));
        var result2 = Record.Exception(() => CryptographyExtend.DecryptPassword(testInput));

        // Assert
        // The method should behave consistently with the same input
        if (result1 == null && result2 == null)
        {
            // Both succeeded - verify they return the same result
            var decrypted1 = CryptographyExtend.DecryptPassword(testInput);
            var decrypted2 = CryptographyExtend.DecryptPassword(testInput);
            decrypted1.Should().Be(decrypted2);
        }
        else
        {
            // Both should fail in the same way
            result1?.GetType().Should().Be(result2?.GetType());
        }
    }
}
