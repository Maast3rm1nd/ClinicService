using System.Security.Cryptography;
using System.Text;
using ClinicServiceContext.Entities;
using ClinicServiceDAL;
using Microsoft.EntityFrameworkCore;

namespace WS_ClinicService.Core.Auth
{
    public sealed class TrustedTwoFactorDeviceService
    {
        public const string CookieName = "clinic_trusted_device";
        private static readonly TimeSpan DeviceLifetime = TimeSpan.FromDays(30);
        private readonly ClinicDbContext _dbContext;

        public TrustedTwoFactorDeviceService(ClinicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> TrustAsync(Guid personId, CancellationToken cancellationToken)
        {
            var token = CreateToken();
            var now = DateTimeOffset.UtcNow;
            _dbContext.TrustedTwoFactorDevices.Add(new TrustedTwoFactorDevice
            {
                PersonId = personId,
                TokenHash = Hash(token),
                CreatedAt = now,
                ExpiresAt = now.Add(DeviceLifetime)
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
            return token;
        }

        public async Task<bool> IsTrustedAsync(
            Guid personId,
            string? token,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var tokenHash = Hash(token);
            var device = await _dbContext.TrustedTwoFactorDevices
                .AsNoTracking()
                .SingleOrDefaultAsync(device => device.PersonId == personId
                    && device.TokenHash == tokenHash
                    && device.RevokedAt == null,
                    cancellationToken);

            if (device is null || device.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                return false;
            }

            var updated = await _dbContext.TrustedTwoFactorDevices
                .Where(item => item.Id == device.Id && item.RevokedAt == null)
                .ExecuteUpdateAsync(
                    update => update.SetProperty(item => item.LastUsedAt, DateTimeOffset.UtcNow),
                    cancellationToken);

            return updated == 1;
        }

        public async Task RevokeAllAsync(Guid personId, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;
            await _dbContext.TrustedTwoFactorDevices
                .Where(device => device.PersonId == personId && device.RevokedAt == null)
                .ExecuteUpdateAsync(
                    update => update.SetProperty(device => device.RevokedAt, now),
                    cancellationToken);
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
    }
}
