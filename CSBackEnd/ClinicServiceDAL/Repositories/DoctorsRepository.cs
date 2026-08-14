using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class DoctorsRepository : RepositoryBase<Doctor>, IDoctorsRepository
    {
        public DoctorsRepository(ClinicDbContext context) : base(context)
        {
        }

        public Task AddDoctor(Doctor doctor)
        {
            return AddObject(doctor);
        }

        public Task EditDoctor(Doctor doctor)
        {
            return UpdateObject(doctor);
        }

        public Task<Doctor[]> GetDoctors(Guid[] ids, CancellationToken cancellationToken)
        {
            return GetObjectsByIds(ids, cancellationToken);
        }

        public Task DeleteDoctor(Guid id, CancellationToken cancellationToken)
        {
            return SoftDeleteById(id);
        }
    }
}