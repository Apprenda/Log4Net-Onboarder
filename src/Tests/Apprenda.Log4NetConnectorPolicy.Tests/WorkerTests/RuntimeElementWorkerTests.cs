// -------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeElementWorkerTests.cs" company="Apprenda, Inc.">
//    The MIT License (MIT)
//    
//    Copyright (c) 2018 Apprenda Inc.
//    
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//    
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//    
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.
//  </copyright>
//  --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Apprenda.Log4NetConnectorPolicy.WorkloadUpdate;
using FluentAssertions;
using Xunit;

namespace Apprenda.Log4NetConnectorPolicy.Tests
{
    public class RuntimeElementWorkerTests
    {
        [Fact]
        public void NoChangesWhenVersionsMatch()
        {
            var container = new XElement("configuration");
            ConfigRuntimeBindingRedirectWorker.ModifyConfigurationElement(container, new BindingRedirectSettings
            {
                AssemblyName = "log4net",
                CorrectNamespace = false,
                NewVersion = "1.2.13.0",
                OldVersion = "1.2.13.0"
            }, new List<string>());

            container.ToString().Should().Be(XElement.Parse("<configuration />").ToString());
        }

        [Fact]
        public void WhenWorkloadHasNewerDependency_AndNoRedirection_RedirectionIsAdded()
        {
            var container = XElement.Parse(@"<configuration />");
            var messages = new List<string>();
            var redirectSettings = UpdateRedirectSettings();

            ConfigRuntimeBindingRedirectWorker.ModifyConfigurationElement(container, redirectSettings, messages);

            XNamespace xns = "urn:schemas-microsoft-com:asm.v1";
            XName assemblyEltName = xns + "assemblyBinding";

            var assemblyBindingElement = container.Should().HaveElement("runtime").Which
                .Should().HaveElement(assemblyEltName).Which;
            assemblyBindingElement.Should().HaveAttribute("xmlns", "urn:schemas-microsoft-com:asm.v1");

            var dependElement = assemblyBindingElement.Should()
                .HaveElement(xns + "dependentAssembly").Which;

            dependElement.Should()
                .HaveElement(xns + "bindingRedirect")
                .Which.Should()
                .HaveAttribute("oldVersion", redirectSettings.OldVersion)
                .And.HaveAttribute("newVersion", redirectSettings.NewVersion);

            dependElement.Should().HaveElement(xns + "assemblyIdentity").Which.Should()
                .HaveAttribute("name", redirectSettings.AssemblyName)
                .And.HaveAttribute("culture", redirectSettings.Culture)
                .And.HaveAttribute("publicKeyToken", redirectSettings.PublicKeyToken);

            messages.Should().BeEmpty();
        }

        [Fact]
        public void WhenWorkoadHasNewerDependency_AndConfigRedirectsCapture_NoChangesFound()
        {
            var originalConfig = ComplexConfiguration("1.2.13.0", "2.0.8.0");
            var target = ComplexConfiguration("1.2.13.0", "2.0.8.0");

            ConfigRuntimeBindingRedirectWorker.ModifyConfigurationElement(target, UpdateRedirectSettings(), new List<string>() );

            target.Should().BeEquivalentTo(originalConfig);
        }

        [Fact]
        public void WhenWorkloadHasNewerDependency_AndConfigDoesNotCaptureVersion_OldVersionIsRewritten()
        {
            var configOldVersion = "1.2.13.0";
            var originalConfig = ComplexConfiguration(configOldVersion, "2.0.8.0");
            var target = ComplexConfiguration(configOldVersion, "2.0.8.0");
            var updateSettings = UpdateRedirectSettings();
            updateSettings.OldVersion = "1.2.10.0";
            ConfigRuntimeBindingRedirectWorker.ModifyConfigurationElement(target, updateSettings, new List<string>());
            XNamespace xns = "urn:schemas-microsoft-com:asm.v1";
            XName assemblyEltName = xns + "assemblyBinding";

            var assemblyBindingElement = target.Should().HaveElement("runtime").Which
                .Should().HaveElement(assemblyEltName).Which;
            assemblyBindingElement.Should().HaveAttribute("xmlns", "urn:schemas-microsoft-com:asm.v1");

            var dependElement = assemblyBindingElement.Should()
                .HaveElement(xns+"dependentAssembly").Which;

            dependElement.Should()
                .HaveElement(xns + "bindingRedirect")
                .Which.Should()
                .HaveAttribute("oldVersion", string.Format("{0}-{1}", updateSettings.OldVersion, configOldVersion))
                .And.HaveAttribute("newVersion", updateSettings.NewVersion);

            dependElement.Should().HaveElement(xns + "assemblyIdentity").Which.Should()
                .HaveAttribute("name", updateSettings.AssemblyName)
                .And.HaveAttribute("culture", updateSettings.Culture)
                .And.HaveAttribute("publicKeyToken", updateSettings.PublicKeyToken);


        }

        [Fact]
        public void WhenWcfSampleConfigProvided_ExistingBindingsAreNotModified()
        {
            var pristine = WcfSampleConfiguration();
            var target = WcfSampleConfiguration();
            var settings = UpdateRedirectSettings();

            ConfigRuntimeBindingRedirectWorker.ModifyConfigurationElement(target, settings, new List<string>());
            XNamespace xns = "urn:schemas-microsoft-com:asm.v1";
            XName assemblyEltName = xns + "assemblyBinding";

            target.ToString().Should().BeEquivalentTo(pristine.ToString());

        }

        private XElement ComplexConfiguration(string redirectPattern, string newVersion)
        {
            return XElement.Parse(string.Format(@"
<configuration>
  <runtime>
    <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
      <dependentAssembly>
        <assemblyIdentity name=""log4net"" publicKeyToken=""669e0ddf0bb1aa2a"" culture=""neutral""/>
        <bindingRedirect oldVersion=""{0}"" newVersion=""{1}""/>
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>", redirectPattern, newVersion));
        }

        private BindingRedirectSettings UpdateRedirectSettings()
        {
            return new BindingRedirectSettings()
            {
                AssemblyName = "log4net",
                CorrectNamespace = false,
                OldVersion = "1.2.13.0",
                NewVersion= "2.0.8.0",
                PublicKeyToken = "669e0ddf0bb1aa2a"
            };
        }

        private XElement WcfSampleConfiguration()
        {
            
            return XElement.Parse(Properties.Resources.WcfConfigurationSample);
        }
    }

}
