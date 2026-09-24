using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace WS_ClinicService.Core.Auth
{
    public sealed class TotpService
    {
        public string CreateSecret()
        {
            return Base32Encode(RandomNumberGenerator.GetBytes(20));
        }

        public bool Verify(string secret, string code, DateTimeOffset now)
        {
            if (code.Length != 6 || !int.TryParse(code, NumberStyles.None, CultureInfo.InvariantCulture, out _))
            {
                return false;
            }

            var counter = now.ToUnixTimeSeconds() / 30;
            for (var offset = -1; offset <= 1; offset++)
            {
                if (GenerateCode(secret, counter + offset) == code)
                {
                    return true;
                }
            }

            return false;
        }

        private static string GenerateCode(string secret, long counter)
        {
            var key = Base32Decode(secret);
            Span<byte> counterBytes = stackalloc byte[8];
            System.Buffers.Binary.BinaryPrimitives.WriteInt64BigEndian(counterBytes, counter);
            var hash = HMACSHA1.HashData(key, counterBytes.ToArray());
            var index = hash[^1] & 0x0f;
            var binary = ((hash[index] & 0x7f) << 24)
                | (hash[index + 1] << 16)
                | (hash[index + 2] << 8)
                | hash[index + 3];
            return (binary % 1_000_000).ToString("D6", CultureInfo.InvariantCulture);
        }

        private static string Base32Encode(byte[] data)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var result = new StringBuilder((data.Length * 8 + 4) / 5);
            var buffer = 0;
            var bits = 0;
            foreach (var value in data)
            {
                buffer = (buffer << 8) | value;
                bits += 8;
                while (bits >= 5)
                {
                    result.Append(alphabet[(buffer >> (bits - 5)) & 31]);
                    bits -= 5;
                }
            }
            if (bits > 0)
            {
                result.Append(alphabet[(buffer << (5 - bits)) & 31]);
            }
            return result.ToString();
        }

        private static byte[] Base32Decode(string value)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var output = new List<byte>();
            var buffer = 0;
            var bits = 0;
            foreach (var character in value.ToUpperInvariant())
            {
                var index = alphabet.IndexOf(character);
                if (index < 0)
                {
                    throw new FormatException("Invalid Base32 secret.");
                }
                buffer = (buffer << 5) | index;
                bits += 5;
                if (bits >= 8)
                {
                    output.Add((byte)(buffer >> (bits - 8)));
                    bits -= 8;
                }
            }
            return output.ToArray();
        }
    }
}
