//namespace Fanex.NCB.Shared.FanexDataExtensions.Services
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Diagnostics.CodeAnalysis;
//    using System.Linq;
//    using System.Threading;
//    using System.Threading.Tasks;
//    using Fanex.NCB.Shared.FanexDataExtensions.ObjectDb;

//    [ExcludeFromCodeCoverage(Justification = "The same BaseService, no need to re-write unit tests")]
//    public class DynamicConnectionService : IDynamicConnectionService
//    {
//        public async Task<IEnumerable<T>> QueryAsync<T>(ICriteriaExtend criteria)
//        {
//            ValidateCriteria(criteria);
//            if (!criteria.IsValid())
//            {
//                return Enumerable.Empty<T>();
//            }

//            using (var objectDb = new SqlObjectDbExtend(criteria.ServerIp, criteria.DbName, criteria.GetSettingKey()))
//            {
//                return await objectDb.QueryAsync<T>(criteria, cancellationToken: default);
//            }
//        }

//        public async Task<(IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet, IEnumerable<TThird> thirdResultSet)> FetchMultipleAsync<TFirst, TSecond, TThird>(
//            ICriteriaExtend criteria)
//        {
//            var mappingTypes = new[] { typeof(TFirst), typeof(TSecond), typeof(TThird) };
//            var fetchedData = await FetchMultipleAsync(criteria, mappingTypes, default);
//            var firstResultSet = fetchedData[0].Cast<TFirst>();
//            var secondResultSet = fetchedData[1].Cast<TSecond>();
//            var thirdResultSet = fetchedData[2].Cast<TThird>();

//            return (firstResultSet, secondResultSet, thirdResultSet);
//        }

//        public async Task<(IEnumerable<TFirst> firstResultSet, IEnumerable<TSecond> secondResultSet)> FetchMultipleAsync<TFirst, TSecond>(ICriteriaExtend criteria)
//        {
//            var mappingTypes = new[] { typeof(TFirst), typeof(TSecond) };
//            var fetchedData = await FetchMultipleAsync(criteria, mappingTypes, default);
//            var firstResultSet = fetchedData[0].Cast<TFirst>();
//            var secondResultSet = fetchedData[1].Cast<TSecond>();

//            return (firstResultSet, secondResultSet);
//        }

//        public static async Task<IEnumerable<object>[]> FetchMultipleAsync(
//            ICriteriaExtend criteria,
//            Type[] mappingTypes,
//            CancellationToken cancellationToken)
//        {
//            if (criteria == null)
//            {
//                throw new ArgumentNullException(nameof(criteria));
//            }

//            var result = InitializeFetchMultipleResultSet(mappingTypes);

//            if (!criteria.IsValid())
//            {
//                return result;
//            }

//            using (var objectDb = new SqlObjectDbExtend(criteria.ServerIp, criteria.DbName, criteria.GetSettingKey()))
//            {
//                using (var multiResultReader = await objectDb.QueryMultipleAsync(criteria, cancellationToken: cancellationToken))
//                {
//                    var index = 0;

//                    while (!multiResultReader.IsEmptyOrClosed && index < mappingTypes.Length)
//                    {
//                        result[index] = await multiResultReader.ReadAsync(mappingTypes[index]).ConfigureAwait(false);
//                        index++;
//                    }
//                }
//            }

//            return result;
//        }

//        private static IEnumerable<object>[] InitializeFetchMultipleResultSet(Type[] mappingTypes)
//        {
//            if (mappingTypes == null || mappingTypes.Length == 0)
//            {
//                throw new ArgumentException($"{nameof(mappingTypes)} is null or empty.", nameof(mappingTypes));
//            }

//            if (mappingTypes.ToList().Exists(type => type == null))
//            {
//                throw new ArgumentException($"{nameof(mappingTypes)} must not contain any null value.", nameof(mappingTypes));
//            }

//            var result = new IEnumerable<object>[mappingTypes.Length];

//            for (var index = 0; index < mappingTypes.Length; index++)
//            {
//                result[index] = Enumerable.Empty<object>();
//            }

//            return result;
//        }

//        private static void ValidateCriteria(ICriteriaExtend criteria)
//        {
//            if (criteria == null)
//            {
//                throw new ArgumentNullException(nameof(criteria));
//            }

//            if (string.IsNullOrEmpty(criteria.ServerIp))
//            {
//                throw new ArgumentNullException(criteria.ServerIp);
//            }
//        }
//    }
//}