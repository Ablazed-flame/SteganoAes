using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionHelper
{
    // You can adjust this depending on performance/security trade-off
    private const int KeySize = 32; // 256 bits
    private const int SaltSize = 16; // 128 bits
    private const int Iterations = 100_000;

    public static string Encrypt(string plainText, string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        using Aes aes = Aes.Create();
        aes.GenerateIV();

        using var keyGen = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        aes.Key = keyGen.GetBytes(KeySize);

        using MemoryStream ms = new MemoryStream();
        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (StreamWriter sw = new StreamWriter(cs, Encoding.UTF8))
        {
            sw.Write(plainText);
        }

        // Format: base64(salt):base64(iv):base64(cipherText)
        string raw = Convert.ToBase64String(salt) + ":" +
                     Convert.ToBase64String(aes.IV) + ":" +
                     Convert.ToBase64String(ms.ToArray());

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
    }

    public static string Decrypt(string encryptedPayload, string password)
    {
        if (string.IsNullOrWhiteSpace(encryptedPayload))
            throw new ArgumentException("Payload is empty or null.");

        string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedPayload));
        string[] parts = decoded.Split(':');
        if (parts.Length != 3)
            throw new FormatException("Invalid payload format. Expected 'salt:iv:cipherText'.");

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] iv = Convert.FromBase64String(parts[1]);
        byte[] cipherText = Convert.FromBase64String(parts[2]);

        try
        {
            using Aes aes = Aes.Create();
            using var keyGen = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            aes.Key = keyGen.GetBytes(KeySize);
            aes.IV = iv;

            using MemoryStream ms = new MemoryStream(cipherText);
            using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs, Encoding.UTF8);
            return sr.ReadToEnd();
        }
        catch (CryptographicException)
        {
            throw new InvalidOperationException("Decryption failed. The password may be incorrect.");
        }
    }
}
