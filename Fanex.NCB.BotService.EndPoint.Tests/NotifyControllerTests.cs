namespace Fanex.NCB.BotService.EndPoint.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Configuration;
    using Microsoft.Bot.Schema;
    using NSubstitute;
    using Xunit;

    public class NotifyControllerTests
    {
        private IBotFrameworkHttpAdapter adapter;
        private BotConfiguration botConfig;
        private IConversationService conversationService;

        ////[Fact]
        ////public async Task Send_InValidBotName_SendContinueWithDefaultAppId()
        ////{
        ////    var controller = GetController();

        ////    var botName = string.Empty;
        ////    var conversationId = string.Empty;
        ////    var message = string.Empty;

        ////    //botConfig.Services.

        ////    var conversationGroup = new ConversationGroup
        ////    {
        ////        BotConversationId = string.Empty,
        ////        BotName = string.Empty,
        ////        ConversationId = string.Empty,
        ////        GroupName = string.Empty,
        ////        ChannelId = string.Empty,
        ////        ServiceUrl = string.Empty
        ////    };
        ////    Task t3 = Task.Run(() => {
        ////        var aaa = 1;
        ////    });

        ////    conversationService.GetGroupConversation(Arg.Any<string>(), Arg.Any<string>()).Returns(conversationGroup);
        ////    var x = (BotFrameworkAdapter)adapter;
            
            

        ////    ((BotFrameworkAdapter)adapter).ContinueConversationAsync(Arg.Any<string>(), Arg.Any<ConversationReference>(), Arg.Any<BotCallbackHandler>(),
        ////        Arg.Any<CancellationToken>()).Returns(t3);

        ////    //Actual
        ////    var actual = await controller.Send(botName, conversationId, message);

        ////    //Assert
        ////    Assert.IsType<OkObjectResult>(actual);
            

        ////    await x.Received().ContinueConversationAsync(Arg.Any<string>(), Arg.Any<ConversationReference>(), Arg.Any<BotCallbackHandler>(),
        ////        Arg.Any<CancellationToken>()
        ////        );
        ////}

        ////[Fact]
        ////public async Task Send_ValidBotName_ReturOkObjectResult()
        ////{
        ////    var controller = GetController();

        ////    var botName = string.Empty;
        ////    var conversationId = string.Empty;
        ////    var message = string.Empty;
  
        ////    //Actual
        ////    var actual = await controller.Send(botName, conversationId, message);

        ////    //Assert
        ////    Assert.IsType<OkObjectResult>(actual);

        ////}

        private NotifyController GetController()
        {
            adapter = Substitute.For<IBotFrameworkHttpAdapter>();
            botConfig = Substitute.For<BotConfiguration>();
            conversationService = Substitute.For<IConversationService>();

            return new NotifyController(adapter, botConfig, conversationService);
        }
    }
}
