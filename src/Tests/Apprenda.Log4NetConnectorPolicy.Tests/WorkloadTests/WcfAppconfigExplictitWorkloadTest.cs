using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
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
            using (var sandbox = Use<FileSandbox>())
            {
                var bootstrapPath = sandbox.ProvideDirectory(@"services\service");
                var lastModTime = GetLastModified(bootstrapPath);
                var dllConfigPath = Path.Combine(bootstrapPath, "Wcf-AppConfig-Explicit-l4n208.dll.config");
                var beforeModification = XDocument.Load(dllConfigPath);

                var request = TestRequest(bootstrapPath, ComponentType.WcfService);
                var sut = new DotNetLog4NetConnector();

                var result = sut.Bootstrap(request);
                result.ShouldBeSuccessful();
                var afterBootstrapModTime = GetLastModified(bootstrapPath);
                afterBootstrapModTime.Should().BeAfter(lastModTime);
                new FileInfo(dllConfigPath).LastWriteTime.Should().BeAfter(lastModTime);

                File.Exists(Path.Combine(bootstrapPath, "Wcf-AppConfig-Explicit-l4n208.dllconfig")).Should().BeFalse();
                File.Exists(Path.Combine(bootstrapPath, "log4net.apprenda.dll")).Should().BeTrue();
                var afterModification = XDocument.Load(dllConfigPath);
                var nameManager = new XmlNamespaceManager(afterModification.CreateNavigator().NameTable);
                nameManager.AddNamespace("a", "urn:schemas-microsoft-com:asm.v1");
                var element = afterModification.XPathSelectElement(
                    "/configuration/runtime/a:assemblyBinding/a:dependentAssembly[(a:assemblyIdentity[(@name=\"log4net\" and @publicKeyToken=\"669e0ddf0bb1aa2a\") and @culture=\"neutral\"]) and a:bindingRedirect[@newVersion=\"2.0.8.0\" and @oldVersion=\"1.2.10.0-2.0.7.0\"]]",
                    nameManager);
                element.Should().NotBeNull();

            }
        }
    }
}
