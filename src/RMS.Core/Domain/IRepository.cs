using System;
using System.Collections.Generic;
using System.Linq;

namespace RMS.Core.Domain
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        bool Exists(Guid id);

        void Create(TEntity entity);

        void Create(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        IEnumerable<TEntity> GetAll();

        Maybe<TEntity> GetById(Guid id);

        IQueryable<TEntity> AsNoTracking();
    }
}
