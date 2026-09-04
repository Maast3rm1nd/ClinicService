using ClinicServiceContext.Entities;
using ClinicServiceContext.Enums;
using ClinicServiceDAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace WS_ClinicService.Core.Auth
{
    public sealed class AuthBootstrapper
    {
        private readonly ClinicDbContext _dbContext;
        private readonly IPasswordHasher<PersonSnapshot> _passwordHasher;
        private readonly AuthBootstrapOptions _options;

        public AuthBootstrapper(
            ClinicDbContext dbContext,
            IPasswordHasher<PersonSnapshot> passwordHasher,
            IOptions<AuthBootstrapOptions> options)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _options = options.Value;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.Login)
                && string.IsNullOrWhiteSpace(_options.Password))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_options.Login)
                || string.IsNullOrWhiteSpace(_options.Password))
            {
                throw new InvalidOperationException(
                    "Auth:Bootstrap:Login and Auth:Bootstrap:Password must be configured together.");
            }

            var user = await _dbContext.PersonSnapshots
                .SingleOrDefaultAsync(person => person.Login == _options.Login, cancellationToken);

            if (user is not null)
            {
                if (user.PasswordHash is null)
                {
                    user.PasswordHash = _passwordHasher.HashPassword(user, _options.Password);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                return;
            }

            user = new Administrator
            {
                FullName = _options.FullName,
                Login = _options.Login,
                PasswordHash = _passwordHasher.HashPassword(
                    new PersonSnapshot { Login = _options.Login },
                    _options.Password),
                IsCurrent = true,
                IsDeleted = false
            };

            await _dbContext.PersonSnapshots.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
