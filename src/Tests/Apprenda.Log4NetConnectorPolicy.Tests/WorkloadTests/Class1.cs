using System.IO;
using Apprenda.API.Extension.Bootstrapping;
using F2F.Sandbox;
using FluentAssertions;
using Xunit;

namespace Apprenda.Log4NetConnectorPolicy.Tests
{
    public class WcfAppconfigExplictitWorkloadTest : AbstractBootstrapPolicyTestFixture
    {
        public WcfAppconfigExplictitWorkloadTest()
        {
            UseAssemblyArchiveStream(ResourceNames.WcfWithConfigSectionExplicitUpdated);
        }

        [Fact]
        public void WhenWorkloadBootstrapped_Success()
        {
            var sandbox = Use<FileSandbox>();
            var bootstrapPath = sandbox.ProvideDirectory(@"services\service");
            var lastModTime = GetLastModified(bootstrapPath);
            var request = TestRequest(bootstrapPath, ComponentType.WcfService);
            var sut = new DotNetLog4NetConnector();

            var result = sut.Bootstrap(request);
            result.ShouldBeSuccessful();
            var afterBootstrapModTime = GetLastModified(bootstrapPath);
            afterBootstrapModTime.Should().BeAfter(lastModTime);
            new FileInfo(Path.Combine(bootstrapPath, "Wcf-AppConfig-Explicit-l4n208.dll.config")).LastWriteTime.Should().BeAfter(lastModTime);

            File.Exists(Path.Combine(bootstrapPath, "Wcf-AppConfig-Explicit-l4n208.dllconfig")).Should().BeFalse();
            File.Exists(Path.Combine(bootstrapPath, "log4net.apprenda.dll")).Should().BeTrue();
        }
    }
}
