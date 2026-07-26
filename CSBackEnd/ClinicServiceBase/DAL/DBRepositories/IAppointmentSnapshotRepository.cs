using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface IAppointmentSnapshotRepository : IClinicServiceRepository<AppointmentSnapshot>
    {
        Task AddAppointment(AppointmentSnapshot appointment);

        Task EditAppointment(AppointmentSnapshot appointment);

        Task<AppointmentSnapshot[]> GetAppointments(Guid[] ids, CancellationToken cancellationToken);

        Task DeleteAppointment(Guid id, CancellationToken cancellationToken);
    }
}
