using System.Security.Cryptography;

namespace e_taraba.API.Services
{
    public class HashHMACSHA256 : IHash
    {
        public void Generate(string password, out byte[] hashPassword, out byte[] hashSalt)
        {
            using(var hmac = new HMACSHA256())
            {
                hashSalt = hmac.Key;
                hashPassword = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool Validate(string password, byte[] hashPassword, byte[] hashSalt)
        {
            using(var hmac = new HMACSHA256(hashSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computedHash.SequenceEqual(hashPassword);
            }
        }
    }
}
