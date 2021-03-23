using System.IO;
using System.Threading.Tasks;

namespace DpdtInject.Extension.Helper
{
    public static class FileHelper
    {
        public static async Task<string> ReadAllTextAsync(
            this string filePath
            )
        {
            using (var reader = File.OpenText(filePath))
            {
                var fileText = await reader.ReadToEndAsync();

                return fileText;
            }
        }
    }
}
