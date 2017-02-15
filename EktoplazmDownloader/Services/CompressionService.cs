using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EktoplazmExtractor.Services
{
    class CompressionService
    {
        internal void Decompress(string zipFile, string extractionDirectory)
        {
            if (Directory.Exists(extractionDirectory) == false)
            {
                Directory.CreateDirectory(extractionDirectory);
            }

            ZipFile.ExtractToDirectory(zipFile, extractionDirectory);
        }
    }
}