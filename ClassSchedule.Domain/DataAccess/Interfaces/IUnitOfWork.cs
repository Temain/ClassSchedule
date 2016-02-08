using ClassSchedule.Domain.DataAccess.Repositories;

namespace ClassSchedule.Domain.DataAccess.Interfaces
{
    public interface IUnitOfWork
    {
        GenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        void Save();
        void Dispose(bool disposing);
        void Dispose();
    }
}