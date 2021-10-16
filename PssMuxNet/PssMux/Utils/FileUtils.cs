using System.IO;

namespace PssMux.Utils
{
    public static class FileUtils
    {
        public static string FileNameAppend(string fileName, string append)
        {
            var targetFileInfo = new FileInfo(fileName);
            return $"{targetFileInfo.DirectoryName}/{targetFileInfo.Name.Replace(targetFileInfo.Extension, "")}_{append}{targetFileInfo.Extension}";
        }
    }
}