using System.Linq.Expressions;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface IClinicServiseRepository
    {
    }

    public interface IClinicServiceRepository<TObj> : IClinicServiseRepository where TObj : class
    {

        public Task<TObj[]> GetAllObjects(CancellationToken cancellationToken);
        
        public Task<TObj[]> GetObjectsByIds(Guid[] ids, CancellationToken cancellationToken);

        public Task<TObj> GetObjectsById(Guid id, CancellationToken cancellationToken);

        public Task<TObj> GetObjectsByFilter(Expression<Func<TObj, bool>> filter, CancellationToken cancellationToken);

        public Task AddObject(TObj obj);

        public Task UpdateObject(TObj obj, TObj dto = null);

        public Task DeleteObject(TObj obj);

        public Task DeleteObjectById(Guid id);

        public Task SoftDeleteById(Guid Id);

        public Task SoftDeleteByFilter(Expression<Func<TObj, bool>> filter);
    }
}
