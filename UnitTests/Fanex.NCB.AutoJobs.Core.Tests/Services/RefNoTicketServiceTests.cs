namespace Fanex.NCB.AutoJobs.Core.Tests.Services;

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.AutoJobs.Core.Models.SpecialDeleteTicket.TicketInfo;
using Fanex.NCB.AutoJobs.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

public class RefNoTicketServiceTests
{
    private readonly IFixture fixture = new Fixture();
    private readonly IConfiguration configuration;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly RefNoTicketService service;

    public RefNoTicketServiceTests()
    {
        configuration = Substitute.For<IConfiguration>();
        httpClientFactory = Substitute.For<IHttpClientFactory>();
        service = new RefNoTicketService(configuration, httpClientFactory);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var service = new RefNoTicketService(configuration, httpClientFactory);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullConfiguration_CreatesInstance()
    {
        // Arrange & Act
        var service = new RefNoTicketService(null, httpClientFactory);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullHttpClientFactory_CreatesInstance()
    {
        // Arrange & Act
        var service = new RefNoTicketService(configuration, null);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTicketByRefNoAsync_WithValidRefNo_ReturnsTicketInfo()
    {
        // Arrange
        var refNo = 12345L;
        var expectedTicket = fixture.Create<TicketInfo>();
        var httpClient = CreateMockHttpClient(expectedTicket);

        SetupConfiguration();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act
        var result = await service.GetTicketByRefNoAsync(refNo);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTicketByRefNoAsync_WithHttpRequestException_ThrowsHttpRequestException()
    {
        // Arrange
        var refNo = 12345L;
        var httpClient = CreateMockHttpClientWithException();

        SetupConfiguration();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => service.GetTicketByRefNoAsync(refNo));
        exception.Message.Should().Contain($"Error fetching ticket by refno {refNo}");
    }

    [Fact]
    public async Task GetTransInfoAsync_WithValidRefNo_ReturnsTransInfo()
    {
        // Arrange
        var refNo = 12345L;
        var ticketInfo = new TicketInfo
        {
            ErrorCode = "0",
            Description = "Test description",
            Odds = "Test odds"
        };
        var httpClient = CreateMockHttpClient(ticketInfo);

        SetupConfiguration();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act
        var (resultRefNo, content) = await service.GetTransInfoAsync(refNo);

        // Assert
        resultRefNo.Should().Be(refNo);
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetTransInfoAsync_WithNullTicketInfo_ReturnsEmptyContent()
    {
        // Arrange
        var refNo = 12345L;
        var httpClient = CreateMockHttpClientWithNullResponse();

        SetupConfiguration();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act
        var (resultRefNo, content) = await service.GetTransInfoAsync(refNo);

        // Assert
        resultRefNo.Should().Be(refNo);
        content.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTransInfoAsync_WithErrorCode_ReturnsEmptyContent()
    {
        // Arrange
        var refNo = 12345L;
        var ticketInfo = new TicketInfo
        {
            ErrorCode = "1", // Error code not "0"
            Description = "Error description"
        };
        var httpClient = CreateMockHttpClient(ticketInfo);

        SetupConfiguration();
        httpClientFactory.CreateClient().Returns(httpClient);

        // Act
        var (resultRefNo, content) = await service.GetTransInfoAsync(refNo);

        // Assert
        resultRefNo.Should().Be(refNo);
        content.Should().BeEmpty();
    }

    private void SetupConfiguration()
    {
        var napUrlSection = Substitute.For<IConfigurationSection>();
        napUrlSection.Value.Returns("https://test.com/api/");
        configuration.GetSection("NapSettings:Url").Returns(napUrlSection);

        var apiKeySection = Substitute.For<IConfigurationSection>();
        apiKeySection.Value.Returns("dGVzdC1hcGkta2V5"); // base64 encoded "test-api-key"
        configuration.GetSection("NapSettings:APIKey").Returns(apiKeySection);
    }

    private HttpClient CreateMockHttpClient(TicketInfo ticketInfo)
    {
        var handler = new MockHttpMessageHandler();
        handler.SetupResponse(ticketInfo);
        return new HttpClient(handler);
    }

    private HttpClient CreateMockHttpClientWithNullResponse()
    {
        var handler = new MockHttpMessageHandler();
        handler.SetupNullResponse();
        return new HttpClient(handler);
    }

    private HttpClient CreateMockHttpClientWithException()
    {
        var handler = new MockHttpMessageHandler();
        handler.SetupException();
        return new HttpClient(handler);
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private TicketInfo responseTicket;
        private bool shouldReturnNull;
        private bool shouldThrowException;

        public void SetupResponse(TicketInfo ticket)
        {
            responseTicket = ticket;
            shouldReturnNull = false;
            shouldThrowException = false;
        }

        public void SetupNullResponse()
        {
            shouldReturnNull = true;
            shouldThrowException = false;
        }

        public void SetupException()
        {
            shouldThrowException = true;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (shouldThrowException)
            {
                throw new HttpRequestException("Test exception");
            }

            if (shouldReturnNull)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("")
                };
            }

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(responseTicket);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
        }
    }
}