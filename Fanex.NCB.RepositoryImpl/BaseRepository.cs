namespace Fanex.NCB.RepositoryImpl
{
    using Fanex.Data.Repository;
    using Fanex.NCB.Criteria;
    using Fanex.NCB.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class BaseRepository
    {
        private readonly IDynamicRepository dynamicRepository;

        protected BaseRepository(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchAsync<T>(criteria).ConfigureAwait(false);
            }

            return new List<T>();
        }

        protected async Task<(IEnumerable<TFirst>, IEnumerable<TSecond>)> FetchMultipleAsync<TFirst, TSecond>(ICriteria criteria)
        {
            return await dynamicRepository.FetchMultipleAsync<TFirst, TSecond>(criteria).ConfigureAwait(false);
        }

        protected async Task<(IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>)> FetchMultipleAsync<TFirst, TSecond, TThird>(ICriteria criteria)
        {
            return await dynamicRepository.FetchMultipleAsync<TFirst, TSecond, TThird>(criteria).ConfigureAwait(false);
        }

        protected async Task<T> GetDataEntityAsync<T>(ICriteria criteria)
        {
            return await dynamicRepository.GetAsync<T>(criteria).ConfigureAwait(false);
        }

        protected async Task<CommandResult> ExecuteNonQueryAsync(BaseCommand command)
        {
            await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);

            return new CommandResult
            {
                Code = command.ErrCode,
                Message = command.ErrMsg
            };
        }
    }
}