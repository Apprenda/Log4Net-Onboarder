using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apprenda.API.Extension.Bootstrapping;
using Apprenda.API.Extension.CustomProperties;
using F2F.Sandbox;
using F2F.Testing.Xunit;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Apprenda.Log4NetConnectorPolicy.Tests
{
    public class InvalidWorkloadTests : AbstractBootstrapPolicyTestFixture
    {
        public InvalidWorkloadTests()
        {
            UseAssemblyArchiveStream(ResourceNames.AspWithoutLog4Net);
        }

        [Fact]
        public void WhenBootstrapperRequest_IsNotNetFXWorkload_ReturnsSuccess()
        {
            var request = new BootstrappingRequest(string.Empty, ComponentType.War, new Guid(), new Guid(), string.Empty, string.Empty, string.Empty, new CustomProperty[0]);

            var result = new DotNetLog4NetConnector().Bootstrap(request);

            result.ShouldBeSuccessful();
        }

        [Fact]
        public void WhenBootstrapperRequest_IsWindowsService_ReturnsSuccess()
        {
            var request = new BootstrappingRequest(string.Empty, ComponentType.WindowsService, new Guid(), new Guid(), string.Empty, string.Empty, string.Empty, new CustomProperty[0]);

            var result = new DotNetLog4NetConnector().Bootstrap(request);

            result.ShouldBeSuccessful();
        }

        [Fact]
        public void WhenBootstrapperRequest_IsUiWithoutLog4Net_ReturnsSuccess()
        {
            var sandbox = Use<FileSandbox>();
            var bootstrapPath = sandbox.ProvideDirectory("interfaces/root");
            var request = TestRequest(bootstrapPath, ComponentType.AspNet);
            var sut = new DotNetLog4NetConnector();
            var result = sut.Bootstrap(request);
            result.ShouldBeSuccessful();
        }

        [Fact]
        public void WhenBootstrapperRequest_IsUiWithoutLog4Net_FilesAreUnchanged()
        {
            var sandbox = Use<FileSandbox>();
            var bootstrapPath = sandbox.ProvideDirectory("interfaces/root");
            var lastModTime = GetLastModified(bootstrapPath);
            var request = TestRequest(bootstrapPath, ComponentType.AspNet);
            var sut = new DotNetLog4NetConnector();
            var result = sut.Bootstrap(request);
            result.ShouldBeSuccessful();
            var afterBootstrapModTime = GetLastModified(bootstrapPath);
            afterBootstrapModTime.Should().Be(lastModTime);
        }
    }
}
