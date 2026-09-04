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

        public DatabaseAuthenticationService(
            ClinicDbContext dbContext,
            IPasswordHasher<PersonSnapshot> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
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

            if (user?.PasswordHash is null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded
                ? user
                : null;
        }

        public string HashPassword(PersonSnapshot user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }
    }
}
