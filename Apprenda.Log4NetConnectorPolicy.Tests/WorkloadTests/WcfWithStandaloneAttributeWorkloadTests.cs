using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apprenda.API.Extension.Bootstrapping;
using F2F.Sandbox;
using FluentAssertions;
using Xunit;

namespace Apprenda.Log4NetConnectorPolicy.Tests
{
    public class WcfWithStandaloneAttributeWorkloadTests : AbstractBootstrapPolicyTestFixture
    {
        public WcfWithStandaloneAttributeWorkloadTests()
        {
            UseAssemblyArchiveStream(ResourceNames.WcfWithStandaloneAttribute);
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
            new FileInfo(Path.Combine(bootstrapPath, "default.log4net")).LastWriteTime.Should().BeAfter(lastModTime);
            File.Exists(Path.Combine(bootstrapPath, "log4net.apprenda.dll")).Should().BeTrue();
        }
    }
}
