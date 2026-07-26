using ClinicServiceContext.Entities;

namespace ClinicServiceBase.DAL.DBRepositories
{
    public interface IScheduleRepository : IClinicServiceRepository<Schedule>
    {
        Task AddSchedule(Schedule schedule);

        Task EditSchedule(Schedule schedule);

        Task<Schedule> GetSchedules(Guid[] ids, CancellationToken cancellationToken);

        Task DeleteSchedule(Guid scheduleId);
    }
}
