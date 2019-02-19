using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Fanex.NCB.Bot.BaseStorage
{
    public interface IBaseRepository<T> where T : class
    {
        bool Insert(T value);
        bool Update(T newValue);
        T Single(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Fetch(Expression<Func<T, bool>> predicate);
    }
}
