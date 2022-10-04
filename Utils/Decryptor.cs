using System.Security.Cryptography;
using System.Text;

namespace IssueTracker.Utils;

public class Decryptor
{
    public static string Decrypt(string connectionString)
    {
        using var des = Aes.Create();
        try
        {
            const string publicKey = "12345678";
            const string secretKey = "87654321";
            var privateKeyByte = Encoding.UTF8.GetBytes(secretKey);
            var publicKeyByte = Encoding.UTF8.GetBytes(publicKey);
            var inputByteArray = Convert.FromBase64String(connectionString.Replace(" ", "+"));
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(publicKeyByte, privateKeyByte), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var encoding = Encoding.UTF8;
            var toReturn = encoding.GetString(ms.ToArray());
            return toReturn;
        }
        catch (Exception ae)
        {
            throw new Exception(ae.Message, ae.InnerException);
        }
    }
}