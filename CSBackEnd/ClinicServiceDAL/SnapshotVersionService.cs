using ClinicServiceContext.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ClinicServiceDAL
{
    public sealed class SnapshotVersionService(ClinicDbContext context)
    {
        public async Task<TEntity> CreateVersionAsync<TEntity>(
            Guid id,
            Action<TEntity> applyChanges,
            Guid? changedBy,
            CancellationToken cancellationToken)
            where TEntity : SnapshotBase
        {
            var current = await context.Set<TEntity>()
                .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken)
                ?? throw new InvalidOperationException($"Current snapshot [{id}] was not found.");

            var next = Clone(current);
            next.Id = Guid.NewGuid();
            next.Version = checked(current.Version + 1);
            next.ValidFrom = DateTimeOffset.UtcNow;
            next.ValidTo = null;
            next.IsCurrent = true;
            next.IsDeleted = false;
            next.EditDateTime = next.ValidFrom;
            next.ChangedBy = changedBy;

            applyChanges(next);

            current.IsCurrent = false;
            current.ValidTo = next.ValidFrom;
            current.EditDateTime = next.ValidFrom;
            current.ChangedBy = changedBy;

            context.Set<TEntity>().Add(next);
            return next;
        }

        public async Task<bool> DeleteAsync<TEntity>(
            Guid id,
            Guid? changedBy,
            CancellationToken cancellationToken = default)
            where TEntity : SnapshotBase
        {
            var current = await context.Set<TEntity>()
                .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

            if (current is null)
            {
                return false;
            }

            var now = DateTimeOffset.UtcNow;
            current.IsCurrent = false;
            current.IsDeleted = true;
            current.ValidTo = now;
            current.EditDateTime = now;
            current.ChangedBy = changedBy;
            return true;
        }

        public async Task<TEntity?> RestoreAsync<TEntity>(
            Guid entityId,
            Guid? changedBy,
            CancellationToken cancellationToken = default)
            where TEntity : SnapshotBase
        {
            var latest = await context.Set<TEntity>()
                .IgnoreQueryFilters()
                .Where(entity => entity.EntityId == entityId && entity.IsDeleted)
                .OrderByDescending(entity => entity.Version)
                .FirstOrDefaultAsync(cancellationToken);

            if (latest is null || await context.Set<TEntity>().AnyAsync(
                    entity => entity.EntityId == entityId && entity.IsCurrent && !entity.IsDeleted,
                    cancellationToken))
            {
                return null;
            }

            var restored = Clone(latest);
            restored.Id = Guid.NewGuid();
            restored.Version = checked(latest.Version + 1);
            restored.ValidFrom = DateTimeOffset.UtcNow;
            restored.ValidTo = null;
            restored.IsCurrent = true;
            restored.IsDeleted = false;
            restored.EditDateTime = restored.ValidFrom;
            restored.ChangedBy = changedBy;

            context.Set<TEntity>().Add(restored);
            return restored;
        }

        public async Task<TEntity> ReplaceVersionAsync<TEntity>(
            TEntity replacement,
            CancellationToken cancellationToken = default)
            where TEntity : SnapshotBase
        {
            return await CreateVersionAsync<TEntity>(replacement.Id, next => CopyValues(replacement, next), replacement.ChangedBy, cancellationToken);
        }

        private static TEntity Clone<TEntity>(TEntity source)
            where TEntity : SnapshotBase
        {
            var clone = Activator.CreateInstance<TEntity>();
            var excluded = new HashSet<string>(StringComparer.Ordinal)
            {
                nameof(Entity.Id),
                nameof(SnapshotBase.Version),
                nameof(SnapshotBase.ValidFrom),
                nameof(SnapshotBase.ValidTo),
                nameof(SnapshotBase.IsCurrent),
                nameof(SnapshotBase.IsDeleted),
                nameof(SnapshotBase.EditDateTime),
                nameof(SnapshotBase.ChangedBy)
            };

            foreach (var property in typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                         .Where(property => property.CanRead && property.CanWrite && !excluded.Contains(property.Name)))
            {
                property.SetValue(clone, property.GetValue(source));
            }

            clone.EntityId = source.EntityId;
            clone.CreationDateTime = source.CreationDateTime;
            return clone;
        }

        private static void CopyValues<TEntity>(TEntity source, TEntity target)
            where TEntity : SnapshotBase
        {
            var excluded = new HashSet<string>(StringComparer.Ordinal)
            {
                nameof(Entity.Id), nameof(SnapshotBase.EntityId), nameof(SnapshotBase.Version),
                nameof(SnapshotBase.ValidFrom), nameof(SnapshotBase.ValidTo), nameof(SnapshotBase.IsCurrent),
                nameof(SnapshotBase.IsDeleted), nameof(SnapshotBase.EditDateTime), nameof(SnapshotBase.ChangedBy),
                nameof(BaseEntity.CreationDateTime)
            };

            foreach (var property in typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                         .Where(property => property.CanRead && property.CanWrite && !excluded.Contains(property.Name)))
            {
                property.SetValue(target, property.GetValue(source));
            }
        }
    }
}
