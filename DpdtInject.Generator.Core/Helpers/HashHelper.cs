using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Core.Helpers
{
    public static class HashHelper
    {
        public static string GetStringSha256Hash(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}
