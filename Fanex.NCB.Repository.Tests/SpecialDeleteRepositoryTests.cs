namespace Fanex.NCB.Repository.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Shared.Models;
    using NSubstitute;
    using Xunit;

    public class SpecialDeleteRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public SpecialDeleteRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        [Fact]
        public async Task ImportMessageAndGetNotification_WhenCalled_ReturnExpectedResult()
        {
            var multiReader = FakeMultipleResultReader();
            var criteria = FakeCriteria();
            var repository = new SpecialDeleteRepository();
            var commandResult = new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };
            multiReader.ReadFirstOrDefault<CommandResult>().Returns(commandResult);
            multiReader.Read<NotificationRecipient>().Returns(new List<NotificationRecipient> { new NotificationRecipient { BotName = "Test" } });
            multiReader.Read<NotificationSendMessage>().Returns(new List<NotificationSendMessage> { new NotificationSendMessage { Content = "ABC", NotificationId = 1 } });
            objectDb.QueryMultipleAsync(criteria).Returns(multiReader);

            //Actual
            var actual = await repository.ImportMessageAndGetNotification(criteria);

            //Assert
            await objectDb.Received(1).QueryMultipleAsync(Arg.Any<ICriteria>());
            Assert.Single(actual.Item2);
            Assert.True(actual.Item1.IsSuccessful);
        }

        #region Helper

        private ICriteria FakeCriteria()
        {
            var criteria = Substitute.For<ICriteria>();
            criteria.GetSettingKey().Returns("NapChatBot_Test");

            return criteria;
        }

        private IMultiResultReader FakeMultipleResultReader()
        {
            return Substitute.For<IMultiResultReader>();
        }

        #endregion Helper
    }
}