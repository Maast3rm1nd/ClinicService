using ClinicServiceContext.Entities;
using ClinicServiceDAL;

namespace WS_ClinicService.Core.Auth
{
    public sealed class SecurityAuditService
    {
        private readonly ClinicDbContext _dbContext;

        public SecurityAuditService(ClinicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task WriteAsync(
            string eventType,
            bool succeeded,
            Guid? personId,
            string? ipAddress,
            string? userAgent,
            string? details,
            CancellationToken cancellationToken)
        {
            _dbContext.SecurityAuditEvents.Add(new SecurityAuditEvent
            {
                EventType = eventType,
                Succeeded = succeeded,
                PersonId = personId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Details = details
            });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
