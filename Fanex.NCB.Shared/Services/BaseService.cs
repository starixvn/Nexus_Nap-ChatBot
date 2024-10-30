namespace Fanex.NCB.Shared.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NAP.Core.Extensions;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    #pragma warning disable S2436
    public class BaseService : IBaseService
    {
        private readonly IDynamicRepository dynamicRepository;

        public BaseService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria, string serverIp = "")
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchAsync<T>(criteria, serverIp);
            }

            return new List<T>();
        }

        public async Task<T> GetDataEntityAsync<T>(ICriteria criteria, string serverIp = "")
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.GetAsync<T>(criteria, serverIp);
            }

            return default;
        }

        public async Task<ValueTuple<IEnumerable<T1>, IEnumerable<T2>>> FetchMultipleAsync<T1, T2>(ICriteria criteria, string serverIp = "")
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<T1, T2>(criteria, serverIp);
            }
            return new ValueTuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());
        }

        public async Task<ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> FetchMultipleAsync<T1, T2, T3>(ICriteria criteria, string serverIp = "")
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<T1, T2, T3>(criteria, serverIp);
            }

            return new ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(new List<T1>(), new List<T2>(), new List<T3>());
        }

        public async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>)> FetchMultipleAsync<T1, T2, T3, T4>(ICriteria criteria, string serverIp = "")
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<T1, T2, T3, T4>(criteria, serverIp);
            }

            return new ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>(new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>());
        }

        public async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>)> FetchMultipleAsync<T1, T2, T3, T4, T5>(ICriteria criteria, string serverIp = "")
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<T1, T2, T3, T4, T5>(criteria, serverIp);
            }

            return new ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>(new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>(), new List<T5>());
        }

        public async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>)>
            FetchMultipleAsync<T1, T2, T3, T4, T5, T6>(ICriteria criteria, string serverIp = "")
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<T1, T2, T3, T4, T5, T6>(criteria, serverIp);
            }

            return new ValueTuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>(
                new List<T1>(), new List<T2>(), new List<T3>(), new List<T4>(), new List<T5>(), new List<T6>());
        }

        public async Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command, string serverIp = "")
        {
            var commandResult = new CommandResult();

            if (command != null && command.IsValid())
            {
                await dynamicRepository.ExecuteAsync(command, serverIp);
                var properties = command.ToPropertyDictionary();
                if (properties.ContainsKey("ErrCode"))
                {
                    commandResult.Code = int.Parse(properties["ErrCode"].ToString());
                    commandResult.Message = properties["ErrMsg"] != null ? properties["ErrMsg"].ToString() : string.Empty;
                }
            }

            return commandResult;
        }

        public T GetDataEntity<T>(ICriteria criteria, string serverIp = "")
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Get<T>(criteria, serverIp);
            }

            return default;
        }
    }
    #pragma warning restore S2436
}