namespace Fanex.NCB.Shared.FanexDataExtensions.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDynamicConnectionService
    {
        Task<IEnumerable<T>> QueryAsync<T>(ICriteriaExtend criteria);

        Task<(IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet)> FetchMultipleAsync<TFirst, TSecond>(ICriteriaExtend criteria);

        Task<(IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet, IEnumerable<TThird> thirdResultSet)> FetchMultipleAsync<TFirst, TSecond, TThird>(
            ICriteriaExtend criteria);
    }
}