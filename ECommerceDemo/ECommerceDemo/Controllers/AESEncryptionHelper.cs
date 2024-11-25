using System.Security.Cryptography;
using System.Text;

public class AesEncryptionHelper
{
    private static readonly string key = "your-secret-key"; // 32 byte'lık bir anahtar olmalı, bu örnekte basitleştirilmiş bir anahtar kullanıyoruz
    private static readonly string iv = "your-iv-vector"; // Initialization Vector (IV) - 16 byte

    // Şifreleme
    public static string Encrypt(string plainText)
    {
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var memoryStream = new System.IO.MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (var writer = new System.IO.StreamWriter(cryptoStream))
                    {
                        writer.Write(plainText);
                    }
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }

    // Şifreyi çözme
    public static string Decrypt(string cipherText)
    {
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var memoryStream = new System.IO.MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (var reader = new System.IO.StreamReader(cryptoStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
