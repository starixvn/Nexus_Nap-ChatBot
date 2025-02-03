namespace Fanex.NCB.Services.Tests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.Data.Repository;
using Logging;
using Fanex.NCB.Core.Models;
using Criteria.SyncData;
using Fanex.NCB.Repository.Interfaces;
using Fanex.NCB.Shared.Models.NapUserControl;
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
    public async Task SyncNapDataForSSOLogin_WhenCalledSuccessed_ExecuteExpectedMethod()
    {
        var service = GetService();
        syncDataRepository.FetchDataFromNapUserControl(Arg.Any<GetDataFromNapUserControlCriteria>())
            .Returns(FakeNapData());

        //Actual
        await service.SyncNapDataForSSOLogin(1);

        //Assert

        await baseService.Received().ExecuteNonQueryAsync(Arg.Any<INonQueryCommand>());
    }

    [Fact]
    public async Task SyncNapDataForSSOLogin_WhenCalledThrowException_WriteLog()
    {
        var service = GetService();
        syncDataRepository.FetchDataFromNapUserControl(Arg.Any<GetDataFromNapUserControlCriteria>())
            .Throws(new Exception());

        //Actual
        await service.SyncNapDataForSSOLogin(1);

        //Assert
        await logger.Received().ErrorAsync(Arg.Any<string>());
    }

    #region Helper

    private NapData FakeNapData()
    {
        var fixture = new Fixture();
        return new NapData
        {
            NapFunctions = fixture.Create<List<NapFunction>>(),
            NapGroups = fixture.Create<List<NapGroup>>(),
            NapGroupFunction = fixture.Create<List<NapGroupFunction>>(),
            NapMenuItems = fixture.Create<List<NapMenuItem>>(),
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
        logger = Substitute.For<ILogger>();
        return new SyncDataService(appSettings, syncDataRepository, baseService, logger);
    }

    #endregion Helper
}