// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UiWorkloadWithAssemblyAttributeTests.cs" company="Apprenda, Inc.">
//   The MIT License (MIT)
//   //   Copyright (c) 2015 Apprenda Inc.
//   //   Permission is hereby granted, free of charge, to any person obtaining a copy
//   //   of this software and associated documentation files (the "Software"), to deal
//   //   in the Software without restriction, including without limitation the rights
//   //   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   //   copies of the Software, and to permit persons to whom the Software is
//   //   furnished to do so, subject to the following conditions:
//   //   The above copyright notice and this permission notice shall be included in all
//   //   copies or substantial portions of the Software.
//   //   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   //   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   //   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   //   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   //   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   //   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//   //   SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Apprenda.Log4NetConnectorPolicy.Tests
{
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using F2F.Sandbox;
    using F2F.Testing.Xunit;
    using FluentAssertions;
    using Xunit;

    public class TestSupportResourcesArePresent
    {
        [Theory, 
        InlineData("Resources.aspnet-config-sectionhandler.zip"), 
        InlineData("Resources.aspnet-config-sectionhandler-netfx4.zip"), 
        InlineData("Resources.aspnet-log4net-attribute-standalone.zip"), 
        InlineData("Resources.aspnet-log4net-explicit-standalone.zip")
        ]
        public void RequiredResourcesArePresent(string resourceName)
        {
            {
                var testType = GetType();

                using (var sut = testType.Assembly.GetManifestResourceStream(testType, resourceName))
                {
                    sut.Should().NotBeNull();
                }
            }
        }
    }

    public class ZipFileLocatorTests : TestFixture
    {
        private ZipArchive _archive;
        public ZipFileLocatorTests()
        {
            this.Register(
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(GetType(), "Resources.aspnet-config-sectionhandler.zip"), 
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
            var candidate = sandbox.ProvideFile("interfaces/root/web.config");
            candidate.Should().EndWith("web.config");

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