using ClinicServiceBase.DAL.DBRepositories;
using ClinicServiceContext.Entities;

namespace ClinicServiceDAL.Repositories
{
    public class ScheduleRepository : RepositoryBase<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(ClinicDbContext context) : base(context)
        {
        }

        public Task AddSchedule(Schedule schedule)
        {
            return AddObject(schedule);
        }

        public Task EditSchedule(Schedule schedule)
        {
            return UpdateObject(schedule);
        }

        public async Task<Schedule?> GetSchedules(Guid[] ids, CancellationToken cancellationToken)
        {
            return (await GetObjectsByIds(ids, cancellationToken)).FirstOrDefault();
        }

        public Task DeleteSchedule(Guid scheduleId)
        {
            return DeleteObjectById(scheduleId);
        }
    }
}