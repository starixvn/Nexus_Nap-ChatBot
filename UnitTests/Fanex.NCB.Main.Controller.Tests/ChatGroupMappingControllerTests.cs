namespace Fanex.NCB.Main.Controllers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Main.ViewModel;
    using Fanex.NCB.Main.ViewModel.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Models.Setting;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.SkypeBot.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;
    using static Fanex.NAP.Core.Business.Constant;

    public class ChatGroupMappingControllerTests
    {
        private IChatGroupMappingService chatGroupMappingService;
        private ISettingService settingService;
        private ICommonService commonService;
        private INapSettings napSettings;
        private INapSession napSession;
        private IUserService userService;

        [Fact]
        public void ChatGroupMappingIndex_WhenCalled_ReturnExpectedView()
        {
            var controller = GetController();

            //Act
            var actual = controller.Index();

            //Assert
            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public async Task GetSetting_WhenCalled_SettingServiceShouldBeExeuted5Times()
        {
            var controller = GetController();

            //Actual
            await controller.GetSettings();

            //Assert
            await settingService.Received(1).GetRecipientsAsync();
            await settingService.Received(1).GetBotNameAsync();
            await settingService.Received(1).GetChatGroupTypeAsync();
            settingService.Received(1).GetAllMappingStatus();
            await settingService.Received(1).GetFilterRecipientTypesAsync();
            await settingService.Received(1).GetLicenseeStatusAsync();
        }

        [Fact]
        public async Task GetSetting_WhenCalled_ReturnExpectedJsonResult()
        {
            var controller = GetController();

            var channels = new List<Models.Common.ChannelInfo>
            {
                new Models.Common.ChannelInfo { ChannelId = 1, ChannelName = "skype" }
            };
            var bots = new List<BotName> { new BotName { Id = 1, Name = "Test", ChannelId = 1, ChannelName = "skype" } };
            var recipientTypes = new List<BaseModel> { new BaseModel { Id = 1, Name = "Test" } };
            var recipients = new List<Recipient> { new Recipient { Id = 2, Name = "", RecipientTypeId = 1, Status = "" } };
            var chatGroupTypes = new List<ChatGroupType> { new ChatGroupType { Id = "2", Name = "Licensee" } };
            var licenseeStatuses = new List<BaseModel> { new BaseModel { Id = 1, Name = "Terminated" } };
            var mappingStatus = new List<BaseModel> { new BaseModel { Id = 1, Name = "Mapped" } };
            commonService.GetConversationChannelAsync().Returns(channels);
            settingService.GetBotNameAsync().Returns(bots);
            settingService.GetFilterRecipientTypesAsync().Returns(recipientTypes);
            settingService.GetRecipientsAsync().Returns(recipients);
            settingService.GetChatGroupTypeAsync().Returns(chatGroupTypes);
            settingService.GetLicenseeStatusAsync().Returns(licenseeStatuses);
            settingService.GetAllMappingStatus().Returns(mappingStatus);

            var expected = new ChatGroupMappingFilterViewModel
            {
                ChannelIds = "1",
                BotIds = "1",
                Channels = new List<MultiSelectViewModel> { new MultiSelectViewModel { Id = 1, Text = "skype", Checked = true, IsLeaf = true } },
                Bots = new List<MultiSelectViewModel>
                {
                    new MultiSelectViewModel {
                        Id = -1, Text = "skype",
                        Checked = true, IsLeaf = false,
                        Items = new List<MultiSelectViewModel>
                        {
                            new MultiSelectViewModel { Id = 1, Text = "Test", Checked = true, IsLeaf = true }
                        }
                    }
                },
                RecipientTypes = recipientTypes,
                Recipients = recipients,
                ChatGroupTypes = chatGroupTypes,
                LicenseeStatuses = licenseeStatuses,
                MappingStatuses = mappingStatus
            };

            //Actual
            var actual = await controller.GetSettings();

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<ChatGroupMappingFilterViewModel>(jsonResult.Value);
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public async Task GetAll_WhenCalled_ReturnExpectedResult()
        {
            var controller = GetController();
            var chatgroupMappingResult = new List<ChatGroupMappingResult>
            {
                new ChatGroupMappingResult { BotName = "1on1Notify", ChatGroupTypeID = 1, ConversationID = "Conversation"}
            };
            var getChapGroupResult = new ValueTuple<IEnumerable<ChatGroupMappingResult>, long>(chatgroupMappingResult, 1000);
            chatGroupMappingService.GetChatGroupMappingListAsync(Arg.Any<GetChapGroupMapping>()).Returns(getChapGroupResult);
            napSettings.SiteId.Returns(103);
            userService.GetPermissionAsync(napSession.UserId, napSettings.SiteId, FunctionKey.ChatGroupMapping)
                .Returns((int)Policy.Host);
            var expected = JsonConvert.SerializeObject(new { data = chatgroupMappingResult, count = 1000, isHost = true });
            //actual
            var result = await controller.GetAll(new GetChapGroupMapping());

            //Assert
            await chatGroupMappingService.Received(1).GetChatGroupMappingListAsync(Arg.Any<GetChapGroupMapping>());
            await userService.Received(1).GetPermissionAsync(8, 103, "ChatGroupMapping");

            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<object>(jsonResult.Value);
            Assert.Equal(expected, JsonConvert.SerializeObject(actual));
        }

        [Fact]
        public async Task Edit_WhenCalled_ReturnJsonCommandResult()
        {
            var controller = GetController();
            var expected = new CommandResult
            {
                Code = 1,
                Message = string.Empty
            };
            var model = new EditChatGroupMapping { CreatorID = 8 };
            chatGroupMappingService.EditAsync(model).Returns(expected);

            //Actual
            var result = await controller.Edit(model);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SearchRecipient_WhenSearch_ReturnJsonResult()
        {
            var controller = GetController();
            var expected = new List<Recipient> { new Recipient { Id = 1, Name = "Test", RecipientTypeId = 2, Status = "" } };
            settingService.SearchRecipientsAsync("Test").Returns(expected);

            //actual
            var result = await controller.SearchRecipient("Test");

            //Assert
            var json = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<Recipient>>(json.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SearchGroups_WhenSearch_ReturnJsonResult()
        {
            var controller = GetController();
            var expected = new List<BaseModel> { new BaseModel { Id = 1, Name = "Test" } };
            settingService.SearchChatGroupAsync("Test").Returns(expected);

            //actual
            var result = await controller.SearchChatGroup("Test");

            //Assert
            var json = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<BaseModel>>(json.Value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task UpdateActive_WhenCall_ReturnsCorrectJson()
        {
            // Arrange
            var controller = GetController();
            var expected = new CommandResult { Code = 0, Message = string.Empty };
            var model = new UpdateActiveConversationGroup { GroupID = 1, IsActive = true };
            chatGroupMappingService.UpdateActiveAsync(model).Returns(expected);

            //Actual
            var result = await controller.UpdateActive(model);

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(expected, actual);
        }

        #region helper

        private ChatGroupMappingController GetController()
        {
            chatGroupMappingService = Substitute.For<IChatGroupMappingService>();
            settingService = Substitute.For<ISettingService>();
            commonService = Substitute.For<ICommonService>();
            napSettings = Substitute.For<INapSettings>();
            napSession = Substitute.For<INapSession>();
            userService = Substitute.For<IUserService>();

            napSession.UserId = 8;

            return new ChatGroupMappingController(chatGroupMappingService, settingService, commonService,
                napSettings, napSession, userService);
        }

        #endregion helper
    }
}