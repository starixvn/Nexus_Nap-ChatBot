namespace Fanex.NCB.Services.Tests;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Helper;
using Interfaces;
using Fanex.NCB.Shared.Models.SpecialDeleteTicket.TicketInfo;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;
using FluentAssertions;

public class RefNoTicketServiceTests
{
    private readonly IFixture fixture = new Fixture();
    private IConfiguration configuration;
    private IHttpClientFactory httpClientFactory;

    [Fact]
    public async Task GetTicketByRefNoAsync_WithValidRefNo_ReturnsTicketInfo()
    {
        // Arrange
        var refNo = 123456L;
        var ticketInfo = fixture.Create<TicketInfo>();
        var jsonResponse = JsonConvert.SerializeObject(ticketInfo);

        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        });
        var httpClient = new HttpClient(httpMessageHandler);

        var service = GetService();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act
        var result = await service.GetTicketByRefNoAsync(refNo);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(ticketInfo);
    }

    [Fact]
    public async Task GetTicketByRefNoAsync_WithEmptyResponse_ReturnsNull()
    {
        // Arrange
        var refNo = 123456L;
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(string.Empty)
        });
        var httpClient = new HttpClient(httpMessageHandler);

        var service = GetService();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act
        var result = await service.GetTicketByRefNoAsync(refNo);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTicketByRefNoAsync_WithHttpError_ThrowsHttpRequestException()
    {
        // Arrange
        var refNo = 123456L;
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        });
        var httpClient = new HttpClient(httpMessageHandler);

        var service = GetService();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetTicketByRefNoAsync(refNo));

        exception.Message.Should().Contain($"Error fetching ticket by refno {refNo}");
    }

    [Fact]
    public async Task GetTransInfoAsync_WithValidTicketInfo_ReturnsFormattedContent()
    {
        // Arrange
        var refNo = 123456L;
        var ticketInfo = new TicketInfo
        {
            ErrorCode = "0",
            Description = "<div>Test Description</div>",
            Odds = "1.5"
        };
        var jsonResponse = JsonConvert.SerializeObject(ticketInfo);

        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        });
        var httpClient = new HttpClient(httpMessageHandler);

        var service = GetService();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act
        var result = await service.GetTransInfoAsync(refNo);

        // Assert
        result.Item1.Should().Be(refNo);
        result.Item2.Should().NotBeEmpty();
        result.Item2.Should().Contain("Odds:");
    }

    [Fact]
    public async Task GetTransInfoAsync_WithNullTicketInfo_ReturnsEmptyContent()
    {
        // Arrange
        var refNo = 123456L;
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(string.Empty)
        });
        var httpClient = new HttpClient(httpMessageHandler);

        var service = GetService();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act
        var result = await service.GetTransInfoAsync(refNo);

        // Assert
        result.Item1.Should().Be(refNo);
        result.Item2.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTransInfoAsync_WithErrorCode_ReturnsEmptyContent()
    {
        // Arrange
        var refNo = 123456L;
        var ticketInfo = new TicketInfo
        {
            ErrorCode = "1",
            Description = "Error occurred"
        };
        var jsonResponse = JsonConvert.SerializeObject(ticketInfo);

        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        });
        var httpClient = new HttpClient(httpMessageHandler);

        var service = GetService();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act
        var result = await service.GetTransInfoAsync(refNo);

        // Assert
        result.Item1.Should().Be(refNo);
        result.Item2.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTransInfoAsync_WithValidTicketInfoButNoOdds_ReturnsContentWithoutOdds()
    {
        // Arrange
        var refNo = 123456L;
        var ticketInfo = new TicketInfo
        {
            ErrorCode = "0",
            Description = "<div>Test Description</div>",
            Odds = null
        };
        var jsonResponse = JsonConvert.SerializeObject(ticketInfo);

        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        });
        var httpClient = new HttpClient(httpMessageHandler);

        var service = GetService();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act
        var result = await service.GetTransInfoAsync(refNo);

        // Assert
        result.Item1.Should().Be(refNo);
        result.Item2.Should().NotBeEmpty();
        result.Item2.Should().NotContain("Odds:");
    }

    private RefNoTicketService GetService()
    {
        configuration = Substitute.For<IConfiguration>();
        httpClientFactory = Substitute.For<IHttpClientFactory>();

        // Setup configuration
        var napUrlSection = Substitute.For<IConfigurationSection>();
        napUrlSection.Value.Returns("https://api.test.com/");
        configuration.GetSection("NapSettings:Url").Returns(napUrlSection);

        var apiKeySection = Substitute.For<IConfigurationSection>();
        apiKeySection.Value.Returns("dGVzdEFwaUtleQ=="); // base64 encoded "testApiKey"
        configuration.GetSection("NapSettings:APIKey").Returns(apiKeySection);

        return new RefNoTicketService(configuration, httpClientFactory);
    }
}
