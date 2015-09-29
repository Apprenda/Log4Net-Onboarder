using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using F2F.Sandbox;

namespace Apprenda.Log4NetConnectorPolicy.Tests
{
    /// <summary>
    /// When provided with a zip archive, stream the files in the archive to a cleaned-after-use location
    /// </summary>
    public class ZipFileLocator : IFileLocator, IDisposable
    {
        private ZipArchive _archive;
        
        public ZipFileLocator(ZipArchive archive)
        {
            _archive = archive;
        }

        private ZipArchive Archive
        {
            get { return _archive; }
        }

        public bool Exists(string fileName)
        {
            string altSeparator = fileName.Replace('/', '\\');
            return Archive.Entries.Any(e => e.FullName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                || Archive.Entries.Any(e => e.FullName.Equals(altSeparator, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<string> EnumeratePath(string path)
        {
            string altSeparator = path.Replace('/', '\\');
            return Archive.Entries.Where(e => e.FullName.StartsWith(path, StringComparison.InvariantCultureIgnoreCase))
                .Union(Archive.Entries.Where(e => e.FullName.StartsWith(altSeparator, StringComparison.InvariantCultureIgnoreCase)))
                .Select(e => e.FullName);
        }

        public void CopyTo(string fileName, string destinationPath)
        {
            if (!Exists(fileName))
            {
                return;
            }

            if (!File.Exists(destinationPath))
            {
                string altSeparator = fileName.Replace('/', '\\');
                Archive.Entries
                    .Where(e => e.FullName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                    .Union(Archive.Entries.Where(e => e.FullName.Equals(altSeparator, StringComparison.InvariantCultureIgnoreCase)))
                    .FirstOrDefault()
                    .ExtractToFile(destinationPath);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_archive != null)
                {
                    _archive.Dispose();
                    _archive = null;
                }
            }
        }
    }
}