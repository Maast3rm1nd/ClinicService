using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class AppointmentSnapshotRepository : RepositoryBase<AppointmentSnapshot>, IAppointmentSnapshotRepository
    {
        public AppointmentSnapshotRepository(ClinicDbContext context) : base(context)
        {
        }

        public Task AddAppointment(AppointmentSnapshot appointment)
        {
            return AddObject(appointment);
        }

        public Task EditAppointment(AppointmentSnapshot appointment)
        {
            return UpdateObject(appointment);
        }

        public Task<AppointmentSnapshot[]> GetAppointments(Guid[] ids, CancellationToken cancellationToken)
        {
            return GetObjectsByIds(ids, cancellationToken);
        }

        public Task DeleteAppointment(Guid id, CancellationToken cancellationToken)
        {
            return SoftDeleteById(id);
        }
    }
}