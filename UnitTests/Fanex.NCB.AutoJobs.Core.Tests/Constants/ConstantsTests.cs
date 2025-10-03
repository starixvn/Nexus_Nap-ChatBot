namespace Fanex.NCB.AutoJobs.Core.Tests.Constants;

using Fanex.NCB.AutoJobs.Core.Constants;
using FluentAssertions;
using Xunit;

public class ConstantsTests
{
    [Fact]
    public void SystemId_ShouldHaveCorrectValue()
    {
        // Arrange & Act
        var value = Constants.SystemId;

        // Assert
        value.Should().Be(-1);
    }

    [Fact]
    public void JobIdentify_ShouldHaveCorrectValue()
    {
        // Arrange & Act
        var value = Constants.JobIdentify;

        // Assert
        value.Should().Be("JobIdentify");
        value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ServiceName_SettlementInternalCSD_ShouldHaveCorrectValue()
    {
        // Arrange & Act
        var value = Constants.ServiceName_SettlementInternalCSD;

        // Assert
        value.Should().Be("Settlement - Internal CSD");
        value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ServiceName_SettlementLicensee_ShouldHaveCorrectValue()
    {
        // Arrange & Act
        var value = Constants.ServiceName_SettlementLicensee;

        // Assert
        value.Should().Be("Settlement - Licensee");
        value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ServiceName_SpecialDeleteInternalCSD_ShouldHaveCorrectValue()
    {
        // Arrange & Act
        var value = Constants.ServiceName_SpecialDeleteInternalCSD;

        // Assert
        value.Should().Be("Special Delete - Internal CSD");
        value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ServiceName_ResultAmendmentInternalCSD_ShouldHaveCorrectValue()
    {
        // Arrange & Act
        var value = Constants.ServiceName_ResultAmendmentInternalCSD;

        // Assert
        value.Should().Be("Result Amendment - Internal CSD");
        value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ServiceName_ResultAmendmentLicensee_ShouldHaveCorrectValue()
    {
        // Arrange & Act
        var value = Constants.ServiceName_ResultAmendmentLicensee;

        // Assert
        value.Should().Be("Result Amendment - Licensee");
        value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Constants_ShouldBeConstant()
    {
        // Arrange & Act
        var systemId1 = Constants.SystemId;
        var systemId2 = Constants.SystemId;
        var jobIdentify1 = Constants.JobIdentify;
        var jobIdentify2 = Constants.JobIdentify;

        // Assert
        systemId1.Should().Be(systemId2);
        jobIdentify1.Should().BeSameAs(jobIdentify2);
    }

    [Fact]
    public void ServiceNames_ShouldBeDifferent()
    {
        // Arrange & Act & Assert
        var serviceNames = new[]
        {
            Constants.ServiceName_SettlementInternalCSD,
            Constants.ServiceName_SettlementLicensee,
            Constants.ServiceName_SpecialDeleteInternalCSD,
            Constants.ServiceName_ResultAmendmentInternalCSD,
            Constants.ServiceName_ResultAmendmentLicensee
        };

        // All service names should be unique
        serviceNames.Should().OnlyHaveUniqueItems();
        serviceNames.Should().AllSatisfy(name => name.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public void ServiceNames_ShouldContainExpectedKeywords()
    {
        // Arrange & Act & Assert
        Constants.ServiceName_SettlementInternalCSD.Should().Contain("Settlement");
        Constants.ServiceName_SettlementInternalCSD.Should().Contain("Internal CSD");

        Constants.ServiceName_SettlementLicensee.Should().Contain("Settlement");
        Constants.ServiceName_SettlementLicensee.Should().Contain("Licensee");

        Constants.ServiceName_SpecialDeleteInternalCSD.Should().Contain("Special Delete");
        Constants.ServiceName_SpecialDeleteInternalCSD.Should().Contain("Internal CSD");

        Constants.ServiceName_ResultAmendmentInternalCSD.Should().Contain("Result Amendment");
        Constants.ServiceName_ResultAmendmentInternalCSD.Should().Contain("Internal CSD");

        Constants.ServiceName_ResultAmendmentLicensee.Should().Contain("Result Amendment");
        Constants.ServiceName_ResultAmendmentLicensee.Should().Contain("Licensee");
    }
}
