using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Apprenda.API.Extension.Bootstrapping;
using Apprenda.API.Extension.CustomProperties;
using F2F.Sandbox;
using F2F.Testing.Xunit;

namespace Apprenda.Log4NetConnectorPolicy.Tests
{
    public abstract class AbstractBootstrapPolicyTestFixture : TestFixture
    {
        protected void UseAssemblyArchiveStream(string resourceName)
        {
            Register(GetType().Assembly.GetManifestResourceStream(GetType(), resourceName));
            Register(new ZipArchive(Use<Stream>()));
            Register(new FileSandbox(new ZipFileLocator(Use<ZipArchive>())));
        }

        protected static BootstrappingRequest TestRequest(string path, ComponentType ctype)
        {
            return TestRequest(path, ctype, new CustomProperty[0]);
        }

        protected static BootstrappingRequest TestRequest(
            string path, 
            ComponentType ctype,
            IEnumerable<CustomProperty> props)
        {
            return new BootstrappingRequest(path, ctype, new Guid(), new Guid(), string.Empty, string.Empty, string.Empty, props);
        }

        protected DateTime GetLastModified(string root)
        {
            // do a deep scan for the highest last modified datetime
            if (!Directory.Exists(root))
            {
                return DateTime.MinValue;
            }

            return Directory.GetDirectories(root)
                .Select(GetLastModified)
                .Union(Directory.GetFiles(root).Select(f => new FileInfo(f).LastWriteTime)).Max();
        }

    }
}