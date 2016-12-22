using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.Components
{
    public class ZipCompresser
    {
        public static bool Compress(string targetFile, string folderToCompress)
        {
            if (string.IsNullOrEmpty(targetFile) || !Directory.Exists(folderToCompress))
            {
                return false;
            }

            (new FastZip()).CreateZip(targetFile, folderToCompress, true, "");

            return true;
        }

        public static bool Compress(string targetFile, DirectoryInfo dir)
        {
            if (dir == null || !dir.Exists)
            {
                return false;
            }

            return Compress(targetFile, dir.FullName);
        }

        public static bool Decompress(string zipFile, string targetDir)
        {

            if (!File.Exists(zipFile) || string.IsNullOrEmpty(targetDir))
            {
                return false;
            }
            if (!Directory.Exists(targetDir))
            {
                var dir = Directory.CreateDirectory(targetDir);
                if (!dir.Exists)
                {
                    return false;
                }
            }

            (new FastZip()).ExtractZip(zipFile, targetDir, FastZip.Overwrite.Always, null, "", "", true);
            return true;
        }

        public static bool Decompress(string targetFile, DirectoryInfo dir)
        {
            return Decompress(targetFile, dir.FullName);
        }
    }
}
