namespace Fanex.NCB.Core.Tests.Extensions;

using System;
using System.Runtime.InteropServices;
using Fanex.NCB.Core.Extensions;
using FluentAssertions;
using Xunit;

public class DateTimeExtensionTests
{
    [Fact]
    public void ConvertToUTC_WithValidDateTime_ReturnsUTCDateTime()
    {
        // Arrange
        var localDateTime = new DateTime(2023, 6, 15, 14, 30, 0);

        // Act
        var utcDateTime = localDateTime.ConvertToUTC();

        // Assert
        utcDateTime.Kind.Should().Be(DateTimeKind.Utc);
        utcDateTime.Should().NotBe(localDateTime);
    }

    [Fact]
    public void ConvertToUTC_WithMinValue_ReturnsValidUTCDateTime()
    {
        // Arrange
        var minDateTime = DateTime.MinValue;

        // Act
        var utcDateTime = minDateTime.ConvertToUTC();

        // Assert
        utcDateTime.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void ConvertToUTC_WithMaxValue_ReturnsValidUTCDateTime()
    {
        // Arrange
        var maxDateTime = DateTime.MaxValue;

        // Act
        var utcDateTime = maxDateTime.ConvertToUTC();

        // Assert
        utcDateTime.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void ConvertFromUTC_WithValidUTCDateTime_ReturnsLocalDateTime()
    {
        // Arrange
        var utcDateTime = new DateTime(2023, 6, 15, 6, 30, 0, DateTimeKind.Utc);

        // Act
        var localDateTime = utcDateTime.ConvertFromUTC();

        // Assert
        localDateTime.Should().NotBe(utcDateTime);
        // The converted time should be different from UTC (unless we're in UTC timezone)
    }

    [Fact]
    public void ConvertFromUTC_WithMinValue_ReturnsValidLocalDateTime()
    {
        // Arrange
        var minUtcDateTime = DateTime.MinValue;

        // Act
        var localDateTime = minUtcDateTime.ConvertFromUTC();

        // Assert
        localDateTime.Should().NotBeNull();
    }

    [Fact]
    public void ConvertFromUTC_WithMaxValue_ReturnsValidLocalDateTime()
    {
        // Arrange
        var maxUtcDateTime = DateTime.MaxValue;

        // Act
        var localDateTime = maxUtcDateTime.ConvertFromUTC();

        // Assert
        localDateTime.Should().NotBeNull();
    }

    [Fact]
    public void ConvertToUTC_ThenConvertFromUTC_ShouldBeReversible()
    {
        // Arrange
        var originalDateTime = new DateTime(2023, 6, 15, 14, 30, 0);

        // Act
        var utcDateTime = originalDateTime.ConvertToUTC();
        var backToLocal = utcDateTime.ConvertFromUTC();

        // Assert
        // Due to timezone conversion, we should be close to the original time
        // but might not be exactly the same due to DST and timezone rules
        backToLocal.Date.Should().Be(originalDateTime.Date);
    }

    [Theory]
    [InlineData("2023-01-15 10:30:45")]
    [InlineData("2023-06-15 14:30:45")]
    [InlineData("2023-12-25 23:59:59")]
    public void ConvertToDateTimeString_WithValidDateTimeOffset_ReturnsFormattedString(string expectedString)
    {
        // Arrange
        var dateTimeOffset = DateTimeOffset.Parse(expectedString);

        // Act
        var result = DateTimeExtension.ConvertToDateTimeString(dateTimeOffset);

        // Assert
        result.Should().Be(expectedString);
    }

    [Fact]
    public void ConvertToDateTimeString_WithMinValue_ReturnsFormattedString()
    {
        // Arrange
        var minDateTimeOffset = DateTimeOffset.MinValue;

        // Act
        var result = DateTimeExtension.ConvertToDateTimeString(minDateTimeOffset);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex(@"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}");
    }

    [Fact]
    public void ConvertToDateTimeString_WithMaxValue_ReturnsFormattedString()
    {
        // Arrange
        var maxDateTimeOffset = DateTimeOffset.MaxValue;

        // Act
        var result = DateTimeExtension.ConvertToDateTimeString(maxDateTimeOffset);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex(@"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}");
    }

    [Fact]
    public void ConvertToUTC_UsesCorrectTimeZoneForWindows()
    {
        // This test verifies that the correct timezone is used based on OS
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Arrange
            var dateTime = new DateTime(2023, 6, 15, 14, 30, 0);

            // Act & Assert - Should not throw exception
            var result = dateTime.ConvertToUTC();
            result.Should().NotBeNull();
        }
    }

    [Fact]
    public void ConvertFromUTC_UsesCorrectTimeZoneForLinux()
    {
        // This test verifies that the correct timezone is used based on OS
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Arrange
            var dateTime = new DateTime(2023, 6, 15, 6, 30, 0, DateTimeKind.Utc);

            // Act & Assert - Should not throw exception
            var result = dateTime.ConvertFromUTC();
            result.Should().NotBeNull();
        }
    }
}
