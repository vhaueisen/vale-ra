using System.Security.Cryptography;
using System.IO;
using System.Text;

public static class DataEncrypt
{
    private static readonly string hashAlgorithm = "SHA1";
    private static readonly int passwordIterations = 1000;
    private static readonly int keySize = 256;

    public static byte[] Encrypt(byte[] plainBytes, string p, string s)
    {
        byte[] saltBytes = Encoding.ASCII.GetBytes(s);
        PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(p, saltBytes, hashAlgorithm, passwordIterations);
        byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
        RijndaelManaged symmetricKey = new RijndaelManaged();
        symmetricKey.Mode = CipherMode.CBC;
        byte[] cipherBytes = null;
        symmetricKey.GenerateIV();
        using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, symmetricKey.IV))
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    memStream.Write(symmetricKey.IV, 0, symmetricKey.IV.Length);
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherBytes = memStream.ToArray();
                    memStream.Close();
                    cryptoStream.Close();
                }
            }
        }
        symmetricKey.Clear();
        return cipherBytes;
    }

    public static byte[] Decrypt(byte[] cipherBytes, string p, string s)
    {
        byte[] saltBytes = Encoding.ASCII.GetBytes(s);

        PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(p, saltBytes, hashAlgorithm, passwordIterations);
        byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
        RijndaelManaged symmetricKey = new RijndaelManaged();
        symmetricKey.Mode = CipherMode.CBC;
        byte[] plainBytes = new byte[cipherBytes.Length];
        using (MemoryStream memStream = new MemoryStream(cipherBytes))
        {
            byte[] initialVectorBytes = new byte[16];
            memStream.Read(initialVectorBytes, 0, 16);
            using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                {
                    cryptoStream.Read(plainBytes, 0, plainBytes.Length);
                    memStream.Close();
                    cryptoStream.Close();
                }
            }
        }
        symmetricKey.Clear();
        return plainBytes;
    }
}