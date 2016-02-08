using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ClassSchedule.Domain.DataAccess.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", int? take = null);

        TEntity GetById(object id);
        void Insert(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        void Update(TEntity entityToUpdate);
        void DeleteRange(Expression<Func<TEntity, bool>> filter = null);
    }
}
