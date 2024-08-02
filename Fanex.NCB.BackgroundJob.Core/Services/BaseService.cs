namespace Fanex.NCB.AutoJobs.Core.Services
{
    using Fanex.Data.Repository;
    using Fanex.NCB.AutoJobs.Core.Models.Core;
    using Fanex.NCB.AutoJobs.Core.Repository;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;

    public class BaseService : IBaseService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBaseRepository baseRepository;

        public BaseService(
            IDynamicRepository dynamicRepository,
            IBaseRepository baseRepository)
        {
            this.dynamicRepository = dynamicRepository;
            this.baseRepository = baseRepository;
        }

        public async Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchAsync<T>(criteria);
            }

            return new List<T>();
        }

        public async Task<ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>>> FetchMultipleAsync<TFirst, TSecond>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<TFirst, TSecond>(criteria);
            }

            return new ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>>(new List<TFirst>(), new List<TSecond>());
        }

        public async Task<ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>> FetchMultipleAsync<TFirst, TSecond, TThird>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<TFirst, TSecond, TThird>(criteria);
            }

            return new ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>(new List<TFirst>(), new List<TSecond>(), new List<TThird>());
        }

        public async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>)> FetchMultipleAsync<T1, T2, T3, T4>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<T1, T2, T3, T4>(criteria);
            }

            return new ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>(new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>());
        }

        public async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>)> FetchMultipleAsync<T1, T2, T3, T4, T5>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<T1, T2, T3, T4, T5>(criteria);
            }

            return new ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>(new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>(), new List<T5>());
        }

        public async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>)>
            FetchMultipleAsync<T1, T2, T3, T4, T5, T6>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<T1, T2, T3, T4, T5, T6>(criteria);
            }

            return new ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>(
                new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>(), new List<T5>(), new List<T6>());
        }

        public async Task<T> GetDataEntityAsync<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.GetAsync<T>(criteria);
            }

            return default;
        }

        public async Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command)
        {
            var commandResult = new CommandResult();

            if (command != null && command.IsValid())
            {
                commandResult = await baseRepository.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }
    }
}