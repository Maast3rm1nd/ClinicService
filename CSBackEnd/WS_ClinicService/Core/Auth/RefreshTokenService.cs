using System.Security.Cryptography;
using System.Text;
using ClinicServiceContext.Entities;
using ClinicServiceDAL;
using Microsoft.EntityFrameworkCore;

namespace WS_ClinicService.Core.Auth
{
    public sealed class RefreshTokenService
    {
        private static readonly TimeSpan Lifetime = TimeSpan.FromDays(30);
        private readonly ClinicDbContext _dbContext;

        public RefreshTokenService(ClinicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(string Token, RefreshSession Session)> CreateAsync(
            Guid personId,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var session = new RefreshSession
            {
                PersonId = personId,
                TokenHash = Hash(token),
                ExpiresAt = DateTimeOffset.UtcNow.Add(Lifetime),
                CreatedByIp = ipAddress,
                UserAgent = userAgent
            };

            _dbContext.RefreshSessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return (token, session);
        }

        public async Task<(string Token, RefreshSession Session)?> RotateAsync(
            string token,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken)
        {
            var hash = Hash(token);
            var session = await _dbContext.RefreshSessions
                .SingleOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);

            if (session is null || session.RevokedAt is not null || session.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                return null;
            }

            var replacement = await CreateAsync(session.PersonId, ipAddress, userAgent, cancellationToken);
            session.RevokedAt = DateTimeOffset.UtcNow;
            session.ReplacedBySessionId = replacement.Session.Id;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return replacement;
        }

        public async Task<bool> RevokeAsync(string token, CancellationToken cancellationToken)
        {
            var session = await _dbContext.RefreshSessions
                .SingleOrDefaultAsync(x => x.TokenHash == Hash(token), cancellationToken);
            if (session is null || session.RevokedAt is not null)
            {
                return false;
            }

            session.RevokedAt = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static string Hash(string token)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }
    }
}
