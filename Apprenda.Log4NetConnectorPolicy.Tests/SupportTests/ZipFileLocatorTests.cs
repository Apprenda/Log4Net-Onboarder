using System.IO;
using System.IO.Compression;
using System.Linq;
using F2F.Sandbox;
using F2F.Testing.Xunit;
using FluentAssertions;
using Xunit;

namespace Apprenda.Log4NetConnectorPolicy.Tests
{
    public class ZipFileLocatorTests : TestFixture
    {
        private ZipArchive _archive;
        public ZipFileLocatorTests()
        {
            var testType = typeof(ZipFileLocatorTests);
            this.Register(
                testType.Assembly
                    .GetManifestResourceStream(testType, "Resources.aspnet-config-sectionhandler.zip"), 
                "ZipStream");
            
            this.Register(_archive = new ZipArchive(Use<Stream>("ZipStream")), "Archive");
            this.Register(new FileSandbox(new ZipFileLocator(this.Use<ZipArchive>("Archive"))));
        }

        [Fact]
        public void FileSandbox_Provides_AllFilesFromArchive()
        {
            using (var sandbox = Use<FileSandbox>())
            {
                foreach (var item in _archive.Entries)
                {
                    var candidate = sandbox.ProvideFile(item.FullName);
                    File.Exists(candidate).Should().BeTrue("Candidate {0} did not exist.", candidate);
                }
            }
        }

        [Fact]
        public void FileSandbox_Provides_WebConfigAndDllsExpected()
        {
            var sandbox = Use<FileSandbox>();
            var candidate = sandbox.ProvideFile("interfaces/root/Web.config");
            candidate.Should().EndWith("Web.config");

            candidate = sandbox.ProvideFile("interfaces/root/bin/log4net.dll");

            candidate.Should().EndWith("log4net.dll");
            sandbox.ExistsFile("interfaces/root/bin/log4net.dll").Should().BeTrue();
            sandbox.ExistsDirectory("interfaces/root/bin").Should().BeTrue();
            var directory = sandbox.ProvideDirectory("interfaces");
            Directory.GetDirectories(directory)
                .Any(p => Directory.Exists(p) && p.EndsWith("root"))
                .Should()
                .BeTrue("interfaces directory provided path {0} but no 'root' was found within.", directory);
        }

    }
}