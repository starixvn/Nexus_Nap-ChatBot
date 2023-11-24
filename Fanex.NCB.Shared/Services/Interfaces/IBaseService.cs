namespace Fanex.NCB.Shared.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Models;

    public interface IBaseService
    {
        Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria, string serverIp = "");

        Task<ValueTuple<IEnumerable<T1>, IEnumerable<T2>>> FetchMultipleAsync<T1, T2>(ICriteria criteria, string serverIp = "");

        Task<ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> FetchMultipleAsync<T1, T2, T3>(ICriteria criteria, string serverIp = "");

        Task<ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>> FetchMultipleAsync<T1, T2, T3, T4>(ICriteria criteria, string serverIp = "");

        Task<ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>> FetchMultipleAsync<T1, T2, T3, T4, T5>(ICriteria criteria, string serverIp = "");

        Task<ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>
            FetchMultipleAsync<T1, T2, T3, T4, T5, T6>(ICriteria criteria, string serverIp = "");

        Task<T> GetDataEntityAsync<T>(ICriteria criteria, string serverIp = "");

        T GetDataEntity<T>(ICriteria criteria, string serverIp = "");

        Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command, string serverIp = "");
    }
}