using System.Security.Cryptography;
using System.Text;
using ClinicServiceBase.Common.Exceptions;
using ClinicServiceContext.Entities;
using ClinicServiceDAL;
using Microsoft.EntityFrameworkCore;

namespace WS_ClinicService.Core.Auth
{
    public sealed class PasswordSetupService
    {
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(24);
        private readonly ClinicDbContext _dbContext;
        private readonly DatabaseAuthenticationService _authenticationService;

        public PasswordSetupService(
            ClinicDbContext dbContext,
            DatabaseAuthenticationService authenticationService)
        {
            _dbContext = dbContext;
            _authenticationService = authenticationService;
        }

        public PasswordSetupInvitation CreateInvitation(Guid personId)
        {
            var now = DateTimeOffset.UtcNow;
            var token = CreateToken();

            _dbContext.PasswordSetupTokens
                .Where(setupToken => setupToken.PersonId == personId && setupToken.UsedAt == null)
                .ExecuteUpdate(update => update.SetProperty(setupToken => setupToken.UsedAt, now));

            var setupToken = new PasswordSetupToken
            {
                PersonId = personId,
                TokenHash = Hash(token),
                CreatedAt = now,
                ExpiresAt = now.Add(TokenLifetime)
            };
            _dbContext.PasswordSetupTokens.Add(setupToken);

            return new PasswordSetupInvitation(token, setupToken.ExpiresAt);
        }

        public async Task<string> ValidateAsync(string token, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw InvalidToken();
            }

            var setupToken = await _dbContext.PasswordSetupTokens
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item => item.TokenHash == Hash(token)
                        && item.UsedAt == null,
                    cancellationToken);

            if (setupToken is null || setupToken.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                throw InvalidToken();
            }

            var person = await _dbContext.PersonSnapshots
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item => item.Id == setupToken.PersonId
                        && item.IsCurrent
                        && !item.IsDeleted
                        && item.PasswordHash == null,
                    cancellationToken);

            return person?.Login ?? throw InvalidToken();
        }

        public async Task CompleteAsync(string token, string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw InvalidToken();
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            {
                throw new UnprocessableEntityException("Password must contain at least 12 characters.");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            var now = DateTimeOffset.UtcNow;
            var tokenHash = Hash(token);
            var setupToken = await _dbContext.PasswordSetupTokens
                .SingleOrDefaultAsync(
                    item => item.TokenHash == tokenHash
                        && item.UsedAt == null,
                    cancellationToken);

            if (setupToken is null || setupToken.ExpiresAt <= now)
            {
                throw InvalidToken();
            }

            var person = await _dbContext.PersonSnapshots
                .SingleOrDefaultAsync(
                    item => item.Id == setupToken.PersonId
                        && item.IsCurrent
                        && !item.IsDeleted
                        && item.PasswordHash == null,
                    cancellationToken);

            if (person is null)
            {
                throw InvalidToken();
            }

            var consumed = await _dbContext.PasswordSetupTokens
                .Where(item => item.Id == setupToken.Id
                    && item.UsedAt == null)
                .ExecuteUpdateAsync(
                    update => update.SetProperty(item => item.UsedAt, now),
                    cancellationToken);

            if (consumed != 1)
            {
                throw InvalidToken();
            }

            person.PasswordHash = _authenticationService.HashPassword(person, password);

            var securityState = await _dbContext.AccountSecurityStates
                .SingleOrDefaultAsync(item => item.PersonId == person.Id, cancellationToken);
            if (securityState is null)
            {
                securityState = new AccountSecurityState { PersonId = person.Id };
                _dbContext.AccountSecurityStates.Add(securityState);
            }
            securityState.PasswordChangedAt = now;
            securityState.EditDateTime = now;

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        private static string CreateToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static string Hash(string token)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }

        private static ConflictException InvalidToken()
        {
            return new ConflictException("Password setup token is invalid, expired, or already used.");
        }
    }

    public sealed record PasswordSetupInvitation(string Token, DateTimeOffset ExpiresAt);
}
