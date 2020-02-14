namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SyncData;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Shared.Models.NapUserControl;
    using Fanex.NCB.Shared.Models.OIC;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;
    using Xunit;

    public class SyncDataServiceTests
    {
        private IOptionsSnapshot<AppSettings> appSettings;
        private ISyncDataRepository syncDataRepository;
        private IBaseService baseService;
        private IMaintenanceService maintenanceService;
        private ILogger logger;
        private IOptionsSnapshot<List<HangfireJobSetting>> jobs;

        [Fact]
        public async Task NapUserControlSyncDataAsync_WhenCalled_ExecuteExpectedMethod()
        {
            var service = GetService();
            syncDataRepository.FetchDataFromNapUserControl(Arg.Any<GetDataFromNapUserControlCriteria>()).Returns(FakeNapData());
            var maintainceTime = new UnderMaintenanceTime
            {
                IsUnderMaintenanceTime = false
            };
            maintenanceService.GetUnderMaintenanceTimeAsync().Returns(maintainceTime);
            //Actual
            await service.NapUserControlSyncDataAsync();

            //Assert
            if (jobs.Value.Any(x => x.JobName.Equals(HangfireJobName.SyncNapUserControlData)))
            {
                await baseService.Received().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
            }
            else
            {
                await baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
            }
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
            await logger.Received().ErrorAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task SyncNapDataFromOIC_WhenCalled_ExecuteExpectedMethod()
        {
            var service = GetService();
            syncDataRepository.FetchDataFromOIC(Arg.Any<GetDataFromOicCriteria>()).Returns(FakeOICData());
            var maintainceTime = new UnderMaintenanceTime
            {
                IsUnderMaintenanceTime = false
            };
            maintenanceService.GetUnderMaintenanceTimeAsync().Returns(maintainceTime);
            //Actual
            await service.OicSyncDataAsync();

            if (jobs.Value.Any(x => x.JobName.Equals(HangfireJobName.SyncOICData)))
            {
                await baseService.Received().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
            }
            else
            {
                await baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
            }
            //Assert
        }

        #region Helper

        private OicData FakeOICData()
        {
            var fixture = new Fixture();
            return new OicData
            {
                Licensees = fixture.Create<List<Licensee>>(),
                LicenseeProducts = fixture.Create<List<LicenseeProduct>>(),
                LicenseeStatuses = fixture.Create<List<LicenseeStatus>>(),
                LicenseeTypes = fixture.Create<List<LicenseeType>>(),
                Products = fixture.Create<List<Product>>()
            };
        }

        private NapData FakeNapData()
        {
            var fixture = new Fixture();
            return new NapData
            {
                NapFunctions = fixture.Create<List<NapFunction>>(),
                NapGroups = fixture.Create<List<NapGroup>>(),
                NapGroupFunction = fixture.Create<List<NapGroupFunction>>(),
                NapMenuItems = fixture.Create<List<NapMenuItem>>(),
                NapOnUser = fixture.Create<List<NapOnUser>>(),
                NapSites = fixture.Create<List<NapSite>>(),
                NapSiteURL = fixture.Create<List<NapSiteURL>>(),
                NapUserFunction = fixture.Create<List<NapUserFunction>>(),
                NapUsers = fixture.Create<List<NapUser>>(),
                NapUserSite = fixture.Create<List<NapUserSite>>(),
                Customer = fixture.Create<List<Customer>>(),
                CustInfo = fixture.Create<List<CustInfo>>()
            };
        }

        private SyncDataService GetService()
        {
            var fixture = new Fixture();

            appSettings = Substitute.For<IOptionsSnapshot<AppSettings>>();
            appSettings.Value.Returns(fixture.Create<AppSettings>());
            syncDataRepository = Substitute.For<ISyncDataRepository>();
            baseService = Substitute.For<IBaseService>();
            maintenanceService = Substitute.For<IMaintenanceService>();
            logger = Substitute.For<ILogger>();
            maintenanceService.IsUnderMaintenanceAsync().Returns(false);
            jobs = Substitute.For<IOptionsSnapshot<List<HangfireJobSetting>>>();
            jobs.Value.Returns(fixture.Create<List<HangfireJobSetting>>());
            return new SyncDataService(appSettings, syncDataRepository, baseService, logger, maintenanceService, jobs);
        }

        #endregion Helper
    }
}