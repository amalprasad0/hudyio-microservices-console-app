using System.Security.Cryptography;
using UserMicroservices.Interface;
namespace UserMicroservices.Helpers
{
    public class Helpers: IHelpers
    {
        public string HashPasskey(string passkey)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passkey));

                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        
    }
}
