using ClinicServiceBase.Common.Exceptions;
using ClinicServiceContext.Entities;
using ClinicServiceDAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WS_ClinicService.Core.Auth
{
    public sealed class DatabaseAuthenticationService
    {
        private readonly ClinicDbContext _dbContext;
        private readonly IPasswordHasher<PersonSnapshot> _passwordHasher;
        private readonly AccountSecurityService _securityService;

        public DatabaseAuthenticationService(
            ClinicDbContext dbContext,
            IPasswordHasher<PersonSnapshot> passwordHasher,
            AccountSecurityService securityService)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _securityService = securityService;
        }

        public async Task<PersonSnapshot?> AuthenticateAsync(
            string login,
            string password,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.PersonSnapshots
                .SingleOrDefaultAsync(
                    person => person.Login == login
                        && person.IsCurrent
                        && !person.IsDeleted,
                    cancellationToken);

            if (user is null
                || user.PasswordHash == null
                || await _securityService.IsLockedAsync(user.Id, cancellationToken))
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result is not (PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded))
            {
                await _securityService.RegisterFailureAsync(user.Id, cancellationToken);
                return null;
            }

            await _securityService.RegisterSuccessAsync(user.Id, cancellationToken);
            return user;
        }

        public string HashPassword(PersonSnapshot user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public const int MinPasswordLength = 12;

        /// <summary>Replaces the password of a user who must change it and clears the "must change" flag.</summary>
        public async Task ChangePasswordAsync(
            PersonSnapshot user,
            string newPassword,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < MinPasswordLength)
            {
                throw new UnprocessableEntityException($"Password must contain at least {MinPasswordLength} characters.");
            }

            if (user.PasswordHash is not null
                && _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, newPassword) is not PasswordVerificationResult.Failed)
            {
                throw new UnprocessableEntityException("New password must differ from the current one.");
            }

            var now = DateTimeOffset.UtcNow;
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

            var state = await _securityService.GetOrCreateAsync(user.Id, cancellationToken);
            state.MustChangePassword = false;
            state.PasswordChangedAt = now;
            await _securityService.SaveAsync(state, cancellationToken);
        }

        public Task<bool> LoginExistsAsync(
            string login,
            Guid? exceptPersonId,
            CancellationToken cancellationToken)
        {
            return _dbContext.PersonSnapshots
                .IgnoreQueryFilters()
                .AnyAsync(person => person.Login == login
                    && person.IsCurrent
                    && !person.IsDeleted
                    && (exceptPersonId == null || person.Id != exceptPersonId),
                    cancellationToken);
        }

        public Task<PersonSnapshot?> GetCurrentUserAsync(Guid personId, CancellationToken cancellationToken)
        {
            return _dbContext.PersonSnapshots
                .SingleOrDefaultAsync(person => person.Id == personId && person.IsCurrent && !person.IsDeleted, cancellationToken);
        }

        public Task<PersonSnapshot?> GetCurrentUserAsync(string? login, CancellationToken cancellationToken)
        {
            return string.IsNullOrWhiteSpace(login)
                ? Task.FromResult<PersonSnapshot?>(null)
                : _dbContext.PersonSnapshots
                    .SingleOrDefaultAsync(person => person.Login == login && person.IsCurrent && !person.IsDeleted, cancellationToken);
        }
    }
}
