using System.Security.Cryptography;
using System.Text;

namespace QuanLySinhVien.Models
{
    public static class HashHelper
    {
        public static string ComputeSha256Hash(string text)
        {
            if (text == null)
                return null;

            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                var hash = sha256.ComputeHash(bytes);
                var builder = new StringBuilder();
                foreach (var b in hash)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
