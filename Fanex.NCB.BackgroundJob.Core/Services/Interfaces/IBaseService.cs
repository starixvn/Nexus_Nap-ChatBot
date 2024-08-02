namespace Fanex.NCB.AutoJobs.Core.Services.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NCB.AutoJobs.Core.Models.Core;

    public interface IBaseService
    {
        Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria);

        Task<ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>>> FetchMultipleAsync<TFirst, TSecond>(ICriteria criteria);

        Task<ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>> FetchMultipleAsync<TFirst, TSecond, TThird>(ICriteria criteria);

        Task<ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>> FetchMultipleAsync<T1, T2, T3, T4>(ICriteria criteria);

        Task<ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>> FetchMultipleAsync<T1, T2, T3, T4, T5>(ICriteria criteria);

        Task<ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>> FetchMultipleAsync<T1, T2, T3, T4, T5, T6>(ICriteria criteria);

        Task<T> GetDataEntityAsync<T>(ICriteria criteria);

        Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command);
    }
}