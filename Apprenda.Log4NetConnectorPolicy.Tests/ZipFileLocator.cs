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

            return Archive.Entries.Any(e => e.FullName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerable<string> EnumeratePath(string path)
        {
            return Archive.Entries.Where(e => e.FullName.StartsWith(path)).Select(e => e.FullName);
        }

        public void CopyTo(string fileName, string destinationPath)
        {
            if (!Exists(fileName))
            {
                return;
            }

            if (!File.Exists(destinationPath))
            {
                Archive.Entries
                    .Single(e => e.FullName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
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