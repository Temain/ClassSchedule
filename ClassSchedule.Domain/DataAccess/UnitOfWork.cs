using System;
using System.Collections.Generic;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.DataAccess.Repositories;
using ClassSchedule.Domain.Models;

namespace ClassSchedule.Domain.DataAccess
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        private Dictionary<string, object> _repositories;
        private Dictionary<string, object> _extendedRepositories;

        private Dictionary<string, Type> _extendedTypes = new Dictionary<string, Type>
        {
            {typeof(Job).Name, typeof(JobRepository)},
            {typeof(Auditorium).Name, typeof(AuditoriumRepository)},
            {typeof(Housing).Name, typeof(HousingRepository)}
        }; 

        public GenericRepository<TEntity> Repository<TEntity>() where TEntity : class 
        {
            if (_repositories == null)
            {
                _repositories = new Dictionary<string, object>();
            }

            if (_extendedRepositories == null)
            {
                _extendedRepositories = new Dictionary<string, object>();
            }

            var type = typeof (TEntity);
            var typeName = type.Name;

            if (_extendedTypes.ContainsKey(typeName))
            {
                if (!_extendedRepositories.ContainsKey(typeName))
                {
                    var extendedType = _extendedTypes[typeName];
                    var repositoryInstance = Activator.CreateInstance(extendedType, _context);
                    _extendedRepositories.Add(typeName, repositoryInstance);
                }

                return (GenericRepository<TEntity>)_extendedRepositories[typeName];
            }

            if (!_repositories.ContainsKey(typeName))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(type), _context);
                _repositories.Add(typeName, repositoryInstance);
            }
            return (GenericRepository<TEntity>)_repositories[typeName];
        }

        public IEnumerable<T> Execute<T>(string query, params object[] parameters)
        {
            return _context.Database.SqlQuery<T>(query, parameters);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool _disposed;

        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
