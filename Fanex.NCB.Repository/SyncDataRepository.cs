namespace Fanex.NCB.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Shared.Models.NapUserControl;

    public class SyncDataRepository : ISyncDataRepository
    {
        private readonly IDynamicRepository dynamicRepository;

        public SyncDataRepository(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<NapData> FetchDataFromNapUserControl(ICriteria criteria)
        {
            using (var dataSet = await dynamicRepository.FetchDataSetAsync(criteria))
            {
                var menuItems = ConvertDataTableToList<NapMenuItem>(dataSet.Tables[3]);
                var onUsers = ConvertDataTableToList<NapOnUser>(dataSet.Tables[4]);
                var userFunctions = ConvertDataTableToList<NapUserFunction>(dataSet.Tables[7]);
                var users = ConvertDataTableToList<NapUser>(dataSet.Tables[8]);
                var userSites = ConvertDataTableToList<NapUserSite>(dataSet.Tables[9]);
                var customers = ConvertDataTableToList<Customer>(dataSet.Tables[10]);
                var custInfo = ConvertDataTableToList<CustInfo>(dataSet.Tables[11]);
                var data = new NapData
                {
                    NapMenuItems = menuItems,
                    NapOnUser = onUsers,
                    NapUserFunction = userFunctions,
                    NapUsers = users,
                    NapUserSite = userSites,
                    Customer = customers,
                    CustInfo = custInfo
                };
                return data;
            }
        }

        private static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name.ToLowerInvariant()))
                    {
                        try
                        {
                            pro.SetValue(objT, row[pro.Name]);
                        }
                        catch { }
                    }
                }
                return objT;
            }).ToList();
        }
    }
}