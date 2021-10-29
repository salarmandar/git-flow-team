using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Bgt.Ocean.Infrastructure.Configuration
{
    public interface IReadOnlyRepository<TEntity> where TEntity : class
    {
        TEntity FindOne(Expression<Func<TEntity, bool>> predicate);
        TEntity FindById(object id);
        IEnumerable<TEntity> FindAll();
        IEnumerable<TEntity> FindAll(Func<TEntity, bool> predicate);

        IQueryable<TEntity> FindAllAsQueryable();
        IQueryable<TEntity> FindAllAsQueryable(Expression<Func<TEntity, bool>> predicate);

        bool Any(Expression<Func<TEntity, bool>> predicate = null);
    }
}
