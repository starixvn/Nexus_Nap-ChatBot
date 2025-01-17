namespace Fanex.NCB.AutoJobs.Core.Tests.Repository
{
    using Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.AutoJobs.Core.Repository;
    using Models.NapUserControl;
    using Models.OIC;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class SyncDataRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public SyncDataRepositoryTests()
        {
            DbSettingProviderManager
                .StartTestingSession();
            objectDb = Substitute.For<IObjectDb>();
            var mockFactory = Substitute.For<CreateObjectDbFunc>();
            mockFactory(Arg.Any<string>(), Arg.Any<int>()).Returns(objectDb);
            ObjectDbFactory.Mock(mockFactory);
            ObjectDbFactory.CreateInstance(Arg.Any<string>()).Returns(objectDb);
        }

        [Fact]
        public async Task FetchDataFromNapUserControl_WhenCalled_ReturnExpectedResult()
        {
            var multiReader = FakeMultipleResultReader();
            var criteria = FakeCriteria();
            var repository = new SyncDataRepository();
            multiReader.Read<NapFunction>().Returns(new List<NapFunction> { new() { FunctionId = 1 } });
            multiReader.Read<NapGroup>().Returns(new List<NapGroup>());
            multiReader.Read<NapGroupFunction>().Returns(new List<NapGroupFunction>());
            multiReader.Read<NapMenuItem>().Returns(new List<NapMenuItem>());
            multiReader.Read<NapOnUser>().Returns(new List<NapOnUser>());
            multiReader.Read<NapSite>().Returns(new List<NapSite>());
            multiReader.Read<NapSiteURL>().Returns(new List<NapSiteURL>());
            multiReader.Read<NapUserFunction>().Returns(new List<NapUserFunction>());
            multiReader.Read<NapUser>().Returns(new List<NapUser>());
            multiReader.Read<NapUserSite>().Returns(new List<NapUserSite>());
            multiReader.Read<Customer>().Returns(new List<Customer>());
            multiReader.Read<CustInfo>().Returns(new List<CustInfo>());
            objectDb.QueryMultipleAsync(criteria).Returns(multiReader);

            //Actual
            var actual = await repository.FetchDataFromNapUserControl(criteria);

            //Assert
            await objectDb.Received(1).QueryMultipleAsync(Arg.Any<ICriteria>());
            Assert.Equal(1, actual.NapFunctions.FirstOrDefault().FunctionId);
        }

        [Fact]
        public async Task FetchDataFromOIC_WhenCalled_ReturnExpectedResult()
        {
            var multiReader = FakeMultipleResultReader();
            var criteria = FakeCriteria();
            var repository = new SyncDataRepository();
            multiReader.Read<Licensee>().Returns(new List<Licensee> { new Licensee { LicenseeId = 1, LicenseeName = "Test" } });
            multiReader.Read<LicenseeStatus>().Returns(new List<LicenseeStatus>());
            multiReader.Read<LicenseeProduct>().Returns(new List<LicenseeProduct>());
            multiReader.Read<LicenseeType>().Returns(new List<LicenseeType>());
            multiReader.Read<Product>().Returns(new List<Product>());
            multiReader.Read<Platform>().Returns(new List<Platform>());

            objectDb.QueryMultipleAsync(criteria).Returns(multiReader);

            //Actual
            var actual = await repository.FetchDataFromOIC(criteria);

            //Assert
            await objectDb.Received(1).QueryMultipleAsync(Arg.Any<ICriteria>());
            Assert.Equal(1, actual.Licensees.FirstOrDefault().LicenseeId);
            Assert.Equal("Test", actual.Licensees.FirstOrDefault().LicenseeName);
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