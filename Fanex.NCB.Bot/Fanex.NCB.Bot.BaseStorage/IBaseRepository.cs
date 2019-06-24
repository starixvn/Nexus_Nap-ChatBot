using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fanex.NCB.Bot.BaseStorage
{
    public interface IBaseRepository<T> where T : class
    {
        Task<int> InsertAsync(T value);

        Task<bool> UpdateAsync(T newValue);

        Task<T> SingleAsync(Expression<Func<T, bool>> predicate);
    }
}