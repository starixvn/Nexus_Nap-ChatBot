namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.NotificationTemplate;
    using Fanex.NCB.Models.NotificationTemplate;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class NotificationTemplaceServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();

            var total = fixture.Create<long>();
            var templates = fixture.Create<IEnumerable<NotificationTemplate>>();
            var notificationTypeId = fixture.Create<int?>();
            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();

            var criteria = new GetTemplatesCriteria();

            baseService.FetchMultipleAsync<long, NotificationTemplate>(Arg.Do<GetTemplatesCriteria>(x => criteria = x))
                .Returns((new List<long> { total }, templates));

            var result = await service.GetAsync(notificationTypeId, pageIndex, pageSize).ConfigureAwait(false);

            Assert.Equal(notificationTypeId, criteria.NotificationTypeId);
            Assert.Equal(pageIndex, criteria.PageIndex);
            Assert.Equal(pageSize, criteria.PageSize);
            Assert.Equal(total, result.Item1);
            Assert.Equal(templates.Count(), result.Item2.Count());
        }

        private NotificationTemplaceService GetService()
        {
            baseService = Substitute.For<IBaseService>();

            return new NotificationTemplaceService(baseService);
        }
    }
}