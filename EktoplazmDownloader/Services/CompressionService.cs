using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace EktoplazmExtractor.Services
{
    class CompressionService
    {
        internal void Decompress(string zipFile, string extractionDirectory)
        {
            try
            {
                if (Directory.Exists(extractionDirectory) == false)
                {
                    Directory.CreateDirectory(extractionDirectory);
                }

                ZipFile.ExtractToDirectory(zipFile, extractionDirectory);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // todo
            }
        }
    }
}