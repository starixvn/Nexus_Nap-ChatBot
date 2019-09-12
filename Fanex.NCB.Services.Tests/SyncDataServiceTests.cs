namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SyncData;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Shared.Models.NapUserControl;
    using Fanex.NCB.Shared.Models.OIC;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    public class SyncDataServiceTests
    {
        private IOptionsSnapshot<AppSettings> appSettings;
        private ISyncDataRepository syncDataRepository;
        private IBaseService baseService;
        private ILogger logger;

        [Fact]
        public async Task NapUserControlSyncDataAsync_WhenCalled_ExecuteExpectedMethod()
        {
            var service = GetService();
            syncDataRepository.FetchDataFromNapUserControl(Arg.Any<GetDataFromNapUserControlCriteria>()).Returns(FakeNapData());

            //Actual
            await service.NapUserControlSyncDataAsync();

            //Assert
            await baseService.Received().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public async Task SyncNapDataForSSOLogin_WhenCalledSuccessed_ExecuteExpectedMethod()
        {
            var service = GetService();
            syncDataRepository.FetchDataFromNapUserControl(Arg.Any<GetDataFromNapUserControlCriteria>()).Returns(FakeNapData());

            //Actual
            await service.SyncNapDataForSSOLogin(1);

            //Assert
            await baseService.Received().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        [Fact]
        public async Task SyncNapDataForSSOLogin_WhenCalledThrowException_WriteLog()
        {
            var service = GetService();
            syncDataRepository.FetchDataFromNapUserControl(Arg.Any<GetDataFromNapUserControlCriteria>()).Throws(new Exception());

            //Actual
            await service.SyncNapDataForSSOLogin(1);

            //Assert
            logger.Received().Error(Arg.Any<string>());
        }

        [Fact]
        public async Task SyncNapDataFromOIC_WhenCalled_ExecuteExpectedMethod()
        {
            var service = GetService();
            syncDataRepository.FetchDataFromOIC(Arg.Any<GetDataFromOICCriteria>()).Returns(FakeOICData());

            //Actual
            await service.OicSyncDataAsync();

            //Assert
            await baseService.Received().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
        }

        #region Helper

        private OicData FakeOICData()
        {
            return new OicData
            {
                Licensees = new List<Licensee>(),
                LicenseeProducts = new List<LicenseeProduct>(),
                LicenseeStatuses = new List<LicenseeStatus>(),
                LicenseeTypes = new List<LicenseeType>(),
                Products = new List<Product>()
            };
        }

        private NapData FakeNapData()
        {
            return new NapData
            {
                NapFunctions = new List<NapFunction>(),
                NapGroups = new List<NapGroup>(),
                NapGroupFunction = new List<NapGroupFunction>(),
                NapMenuItems = new List<NapMenuItem>(),
                NapOnUser = new List<NapOnUser>(),
                NapSites = new List<NapSite>(),
                NapSiteURL = new List<NapSiteURL>(),
                NapUserFunction = new List<NapUserFunction>(),
                NapUsers = new List<NapUser>(),
                NapUserSite = new List<NapUserSite>(),
                Customer = new List<Customer>(),
                CustInfo = new List<CustInfo>()
            };
        }

        private SyncDataService GetService()
        {
            var settings = new AppSettings
            {
                SiteId = 69
            };
            appSettings = Substitute.For<IOptionsSnapshot<AppSettings>>();
            appSettings.Value.Returns(settings);
            syncDataRepository = Substitute.For<ISyncDataRepository>();
            baseService = Substitute.For<IBaseService>();
            logger = Substitute.For<ILogger>();

            return new SyncDataService(appSettings, syncDataRepository, baseService, logger);
        }

        #endregion Helper
    }
}