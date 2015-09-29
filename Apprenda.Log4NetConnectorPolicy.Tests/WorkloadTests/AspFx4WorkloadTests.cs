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
    public class AspFx4WorkloadTests : AbstractBootstrapPolicyTestFixture
    {
        public AspFx4WorkloadTests()
        {
            base.UseAssemblyArchiveStream(ResourceNames.AspFx4WithConfigSectionHandlerExplicit);
        }

        [Fact]
        public void WhenWorkloadBootstrapped_Success()
        {
            var sandbox = Use<FileSandbox>();
            var bootstrapPath = sandbox.ProvideDirectory("interfaces");
            var lastModTime = GetLastModified(bootstrapPath);
            var request = TestRequest(bootstrapPath, ComponentType.AspNet);
            var sut = new DotNetLog4NetConnector();
            
            var result = sut.Bootstrap(request);
            result.ShouldBeSuccessful();
            var afterBootstrapModTime = GetLastModified(bootstrapPath);
            afterBootstrapModTime.Should().BeAfter(lastModTime);
            new FileInfo(Path.Combine(bootstrapPath, "root", "web.config")).LastWriteTime.Should().BeAfter(lastModTime);
            File.Exists(Path.Combine(bootstrapPath, "root", "bin", "log4net.apprenda.dll")).Should().BeTrue();
        }

    }
}
