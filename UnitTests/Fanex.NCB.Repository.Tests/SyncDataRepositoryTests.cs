namespace Fanex.NCB.Repository.Tests
{
    using AutoFixture;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Models.NapUserControl;
    using Newtonsoft.Json;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Xunit;

    public class SyncDataRepositoryTests
    {
        private IDynamicRepository dynamicRepository;
        private static readonly Fixture fixture = new();

        [Fact]
        public async void FetchDataFromNapUserControl_WhenCalledReturnExpectedResult()
        {
            var criteria = Substitute.For<ICriteria>();
            var (datasetResponse, expectedResult) = FakeDataResponse();
            var repository = GetRepository();
            dynamicRepository.FetchDataSetAsync(criteria).Returns(datasetResponse);

            var actual = await repository.FetchDataFromNapUserControl(criteria);

            Assert.Equal(JsonConvert.SerializeObject(expectedResult), JsonConvert.SerializeObject(actual));
        }

        private ValueTuple<DataSet, NapData> FakeDataResponse()
        {
            var data0 = FakeDataTable();
            var data1 = FakeDataTable(); ;
            var data2 = FakeDataTable(); ;
            var data5 = FakeDataTable(); ;
            var data6 = FakeDataTable(); ;
            var menuItems = fixture.Create<List<NapMenuItem>>();
            var onUsers = fixture.Create<List<NapOnUser>>();
            var userFunctions = fixture.Create<List<NapUserFunction>>();
            var users = fixture.Create<List<NapUser>>();
            var userSites = fixture.Create<List<NapUserSite>>();
            var customers = fixture.Create<List<Customer>>();
            var custInfo = fixture.Create<List<CustInfo>>();
            var dataSet = new DataSet();
            dataSet.Tables.Add(data0);
            dataSet.Tables.Add(data1);
            dataSet.Tables.Add(data2);
            dataSet.Tables.Add(ConvertToDataTable(menuItems));
            dataSet.Tables.Add(ConvertToDataTable(onUsers));
            dataSet.Tables.Add(data5);
            dataSet.Tables.Add(data6);
            dataSet.Tables.Add(ConvertToDataTable(userFunctions));
            dataSet.Tables.Add(ConvertToDataTable(users));
            dataSet.Tables.Add(ConvertToDataTable(userSites));
            dataSet.Tables.Add(ConvertToDataTable(customers));
            dataSet.Tables.Add(ConvertToDataTable(custInfo));

            return new ValueTuple<DataSet, NapData>(dataSet, new NapData
            {
                NapMenuItems = menuItems,
                NapOnUser = onUsers,
                NapUserFunction = userFunctions,
                NapUsers = users,
                NapUserSite = userSites,
                Customer = customers,
                CustInfo = custInfo
            });
        }

        private static DataTable ConvertToDataTable<T>(List<T> data)
        {
            DataTable table = new();

            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }

        private static DataTable FakeDataTable()
        {
            var table1 = new DataTable();
            table1.Columns.Add("ID", typeof(int));
            table1.Columns.Add("Name", typeof(string));
            table1.Rows.Add(1, "John");
            table1.Rows.Add(2, "Doe");
            return table1;
        }

        private SyncDataRepository GetRepository()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            return new SyncDataRepository(dynamicRepository);
        }
    }
}