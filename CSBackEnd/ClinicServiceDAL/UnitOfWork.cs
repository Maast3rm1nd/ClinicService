using ClinicServiceBase.DAL.Common;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicServiceDAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicDbContext _context;
        private readonly Dictionary<Type, IClinicServiseRepository> _repositories = new();

        public UnitOfWork(ClinicDbContext context)
        {
            _context = context;
        }

        public void CommitToDB()
        {
            _context.SaveChanges();
        }

        public async Task CommitToDBAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public void CommitWithUpdateTime(bool updateTime)
        {
            ApplyEditTime(updateTime);
            _context.SaveChanges();
        }

        public async Task CommitWithUpdateTimeAsync(bool updateTime, CancellationToken cancellationToken)
        {
            ApplyEditTime(updateTime);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Register(IClinicServiseRepository repository)
        {
            _repositories[repository.GetType()] = repository;
        }

        public T GetRepository<T>() where T : IClinicServiseRepository
        {
            if (!_repositories.TryGetValue(typeof(T), out var repository))
            {
                repository = (IClinicServiseRepository)Activator.CreateInstance(typeof(T), _context)!;
                _repositories[typeof(T)] = repository;
            }

            return (T)repository;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private void ApplyEditTime(bool updateTime)
        {
            if (!updateTime)
            {
                return;
            }

            var now = DateTimeOffset.UtcNow;

            foreach (var entry in _context.ChangeTracker.Entries<IEditableEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.EditDateTime = now;
                }
            }
        }
    }
}