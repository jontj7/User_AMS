using System.Security.Cryptography;

namespace User_AMS.Services;

public class PasswordHasher
{
    // PBKDF2 con SHA256
    public string Hash(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        var result = new byte[48];
        Buffer.BlockCopy(salt, 0, result, 0, 16);
        Buffer.BlockCopy(hash, 0, result, 16, 32);
        return Convert.ToBase64String(result);
    }

    public bool Verify(string password, string hashed)
    {
        var data = Convert.FromBase64String(hashed);
        var salt = new byte[16];
        Buffer.BlockCopy(data, 0, salt, 0, 16);
        var stored = new byte[32];
        Buffer.BlockCopy(data, 16, stored, 0, 32);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var test = pbkdf2.GetBytes(32);
        return CryptographicOperations.FixedTimeEquals(stored, test);
    }
}
