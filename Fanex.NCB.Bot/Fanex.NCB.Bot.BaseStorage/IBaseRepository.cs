using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.Bot.BaseStorage
{
    public interface IBaseRepository<T> where T : class
    {
        Task<int> InsertAsync(T value);
        Task<bool> UpdateAsync(T newValue);
        Task<T> SingleAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FetchAsync(Expression<Func<T, bool>> predicate);
    }
}
