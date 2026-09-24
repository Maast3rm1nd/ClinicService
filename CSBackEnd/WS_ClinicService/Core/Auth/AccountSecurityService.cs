using ClinicServiceContext.Entities;
using ClinicServiceDAL;
using Microsoft.EntityFrameworkCore;

namespace WS_ClinicService.Core.Auth
{
    public sealed class AccountSecurityService
    {
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
        private readonly ClinicDbContext _dbContext;

        public AccountSecurityService(ClinicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AccountSecurityState> GetOrCreateAsync(Guid personId, CancellationToken cancellationToken)
        {
            var state = await _dbContext.AccountSecurityStates
                .SingleOrDefaultAsync(x => x.PersonId == personId, cancellationToken);
            if (state is not null)
            {
                return state;
            }

            state = new AccountSecurityState { PersonId = personId };
            _dbContext.AccountSecurityStates.Add(state);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return state;
        }

        public async Task<bool> IsLockedAsync(Guid personId, CancellationToken cancellationToken)
        {
            var state = await _dbContext.AccountSecurityStates
                .SingleOrDefaultAsync(x => x.PersonId == personId, cancellationToken);
            return state?.LockedUntil > DateTimeOffset.UtcNow;
        }

        public async Task RegisterFailureAsync(Guid personId, CancellationToken cancellationToken)
        {
            var state = await GetOrCreateAsync(personId, cancellationToken);
            state.FailedLoginCount++;
            if (state.FailedLoginCount >= MaxFailedAttempts)
            {
                state.LockedUntil = DateTimeOffset.UtcNow.Add(LockoutDuration);
                state.FailedLoginCount = 0;
            }
            state.EditDateTime = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task SaveAsync(AccountSecurityState state, CancellationToken cancellationToken)
        {
            state.EditDateTime = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RegisterSuccessAsync(Guid personId, CancellationToken cancellationToken)
        {
            var state = await GetOrCreateAsync(personId, cancellationToken);
            state.FailedLoginCount = 0;
            state.LockedUntil = null;
            state.LastLoginAt = DateTimeOffset.UtcNow;
            state.EditDateTime = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
