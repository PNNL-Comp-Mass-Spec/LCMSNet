using System.IO;
using System.Text;

namespace LcmsNet.IO
{
    public static class FileUtilities
    {
        public static bool CheckDuplicateNames(string path)
        {
            return File.Exists(path);
        }

        public static string GetUniqueFileName(string path, string extension)
        {
            var count = 1;
            var uniqueName = new StringBuilder();
            uniqueName.Append(path);
            uniqueName.Append(extension);
            while (CheckDuplicateNames(uniqueName.ToString()))
            {
                uniqueName.Clear();
                uniqueName.Append(path);
                uniqueName.Append("_" + count++);
                uniqueName.Append(extension);
            }
            return uniqueName.ToString();
        }
    }
}
