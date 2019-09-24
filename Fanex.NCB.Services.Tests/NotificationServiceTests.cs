namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.Notification;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class NotificationServiceTests
    {
        private IBaseService baseService;
        private ISendNotificationService sendNotificationService;

        [Fact]
        public async Task CreateAsync_ValidData_ReturnSuccessResult()
        {
            var service = GetService();
            var model = new CreateNotification
            {
                NotificationTypeId = 1
            };

            baseService.ExecuteNonQueryAsync(Arg.Any<CreateNewNotificationCommand>()).Returns(new CommandResult { Code = 0 });

            var result = await service.CreateAsync(model).ConfigureAwait(false);

            Assert.True(result.IsSuccessful);
        }

        private NotificationService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            sendNotificationService = Substitute.For<ISendNotificationService>();

            return new NotificationService(baseService, sendNotificationService);
        }
    }
}