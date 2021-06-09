namespace Fanex.NCB.NAPResolver.Tests.Attributes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.NAPResolver.Attributes;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Caching.Memory;
    using NSubstitute;
    using Xunit;
    using static Fanex.NAP.Core.Business.Constant;

    public class NapChatBotAuthAttributesTests
    {
        private IMemoryCache cache;
        private IUserService userService;
        private INapSettings napSettings;
        private INapSession napSession;

        [Fact]
        public async Task OnAuthorizationAsync_NoPermissionCache_DoExpectedMethods()
        {
            var attribute = Setup();
            var httpContext = new DefaultHttpContext();
            var context = new AuthorizationFilterContext(
                new ActionContext
                {
                    HttpContext = httpContext,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor(),
                },
                new List<IFilterMetadata>());
            await attribute.OnAuthorizationAsync(context);

            Assert.False(attribute.PermissionCache);
            await userService.Received().GetPermissionAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>());
        }

        [Fact]
        public async Task OnAuthorizationAsync_HavePermissionCache_DoExpectedMethods()
        {
            var attribute = Setup();
            attribute.PermissionCache = true;
            var httpContext = new DefaultHttpContext();
            var context = new AuthorizationFilterContext(
                new ActionContext
                {
                    HttpContext = httpContext,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor(),
                },
                new List<IFilterMetadata>());
            await attribute.OnAuthorizationAsync(context);

            Assert.True(attribute.PermissionCache);
            await userService.Received().GetPermissionAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>());
            cache.ReceivedCalls();
        }

        [Fact]
        public async Task OnAuthorizationAsync_HasValidFunctionPolicy_DoExpectedMethods()
        {
            var attribute = Setup();
            attribute.PermissionCache = true;
            attribute.Policy = Policy.Host;
            var httpContext = new DefaultHttpContext();
            var context = new AuthorizationFilterContext(
                new ActionContext
                {
                    HttpContext = httpContext,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor(),
                },
                new List<IFilterMetadata>());
            await attribute.OnAuthorizationAsync(context);

            Assert.True(attribute.PermissionCache);
            Assert.IsType<RedirectResult>(context.Result);
        }

        private NapChatBotAuthAttribute Setup()
        {
            cache = Substitute.For<IMemoryCache>();
            userService = Substitute.For<IUserService>();
            napSettings = Substitute.For<INapSettings>();
            napSettings.CacheDuration.Returns(120);
            napSession = Substitute.For<INapSession>();
            return new NapChatBotAuthAttribute(userService, napSettings, napSession, cache);
        }
    }
}