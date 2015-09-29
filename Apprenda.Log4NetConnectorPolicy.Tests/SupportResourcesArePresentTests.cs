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
    using System.Reflection;
    using FluentAssertions;
    using Xunit;

    public class SupportResourcesArePresentTests
    {
        [Theory,
        InlineData(ResourceNames.AspWithConfigSectionExplicit), 
        InlineData(ResourceNames.AspFx4WithConfigSectionHandlerExplicit), 
        InlineData(ResourceNames.AspWithStandaloneAttribute),
        InlineData(ResourceNames.AspWithStandaloneExplicit),
        InlineData(ResourceNames.WcfWithConfigSectionAttribute),
        InlineData(ResourceNames.WcfWithConfigSectionExplicit),
        InlineData(ResourceNames.WcfWithStandaloneAttribute),
        InlineData(ResourceNames.WcfWithStandaloneExcplicit),
        InlineData(ResourceNames.AspWithConfigSectionAttribute)
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
}