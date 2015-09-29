using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.Sandbox;
using F2F.Testing.Xunit;
using Xunit;

namespace Apprenda.Log4NetConnectorPolicy.Tests
{
    public class WorkloadInspectorTestFixture : TestFixture
    {
        private ZipArchive _archive;

        private ZipFileLocator _zipFileLocator;

        public void AttachArchiveStream(Stream stream)
        {
            _archive = new ZipArchive(stream);
            _zipFileLocator = new ZipFileLocator(_archive);
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ReleaseUnmanagedResources();
            }
        }

        private void ReleaseUnmanagedResources()
        {
            if (_zipFileLocator != null)
            {
                _zipFileLocator.Dispose();
                _zipFileLocator = null;
            }
        }
    }
}
