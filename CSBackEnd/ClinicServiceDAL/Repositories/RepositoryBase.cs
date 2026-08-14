using System.Linq.Expressions;
using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicServiceDAL.Repositories
{
    public abstract class RepositoryBase<TEntity> : IClinicServiceRepository<TEntity> where TEntity : class
    {
        protected readonly ClinicDbContext Context;

        protected readonly DbSet<TEntity> DbSet;

        protected RepositoryBase(ClinicDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public virtual async Task<TEntity[]> GetAllObjects(CancellationToken cancellationToken)
        {
            return await DbSet.AsNoTracking().ToArrayAsync(cancellationToken);
        }

        public virtual async Task<TEntity[]> GetObjectsByIds(Guid[] ids, CancellationToken cancellationToken)
        {
            return await DbSet.AsNoTracking()
                .Where(e => ids.Contains(EF.Property<Guid>(e, "Id")))
                .ToArrayAsync(cancellationToken);
        }

        public virtual async Task<TEntity> GetObjectsById(Guid id, CancellationToken cancellationToken)
        {
            return await DbSet.AsNoTracking()
                .SingleOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
        }

        public virtual async Task<TEntity> GetObjectsByFilter(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(filter, cancellationToken);
        }

        public virtual async Task AddObject(TEntity obj)
        {
            await DbSet.AddAsync(obj);
        }

        public virtual Task UpdateObject(TEntity obj, TEntity? dto = null)
        {
            DbSet.Update(obj);
            return Task.CompletedTask;
        }

        public virtual Task DeleteObject(TEntity obj)
        {
            DbSet.Remove(obj);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteObjectById(Guid id)
        {
            var entity = await DbSet.SingleOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);

            if (entity != null)
            {
                DbSet.Remove(entity);
            }
        }

        public virtual async Task SoftDeleteById(Guid id)
        {
            var entity = await DbSet.SingleOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);

            if (entity == null)
            {
                return;
            }

            if (entity is SnapshotBase snapshot)
            {
                snapshot.IsDeleted = true;
                snapshot.IsCurrent = false;
            }

            if (entity is IEditableEntity editable)
            {
                editable.EditDateTime = DateTimeOffset.UtcNow;
            }

            DbSet.Update(entity);
        }

        public virtual async Task SoftDeleteByFilter(Expression<Func<TEntity, bool>> filter)
        {
            var entities = await DbSet.Where(filter).ToArrayAsync();

            foreach (var entity in entities)
            {
                if (entity is SnapshotBase snapshot)
                {
                    snapshot.IsDeleted = true;
                    snapshot.IsCurrent = false;
                }

                if (entity is IEditableEntity editable)
                {
                    editable.EditDateTime = DateTimeOffset.UtcNow;
                }
            }
        }
    }
}