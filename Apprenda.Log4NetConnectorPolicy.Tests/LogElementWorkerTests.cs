// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogElementWorkerTests.cs" company="Apprenda, Inc.">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Apprenda Inc.
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//   SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using FluentAssertions;

namespace Apprenda.Log4NetConnectorPolicy.Tests
{
    using System.Xml.Linq;
    using Xunit;

    public class LogElementWorkerTests
    {
        [Fact]
        public void InjectedXmlHasProperForm()
        {
            var container = new XElement("log4net");
            LogElementWorker.UpdateLoggingElement(container);
            
            container.ToString().Should().Be(XElement.Parse(@"
<log4net>
    <root>
        <level value='DEBUG'/>
        <appender-ref ref='ApprendaAppender'/>
    </root>
    <appender name='ApprendaAppender' type='log4net.Apprenda.ApprendaBufferingAppender, log4net.Apprenda'/>
</log4net>").ToString());
        }

        [Fact]
        public void InjectedXmlDoesNotRemoveExistingAppenders()
        {
            var container = XElement.Parse(@"
<log4net>
<root>
<level value='WARN' />
<appender-ref ref='FileLog' />
</root>
<appender name='FileLog' type='RollingFileAppender' />
</log4net>");
            LogElementWorker.UpdateLoggingElement(container);

            container.ToString().Should().Be(XElement.Parse(@"
<log4net>
<root>
<level value='DEBUG' />
<appender-ref ref='FileLog' />
<appender-ref ref='ApprendaAppender' />
</root>
<appender name='FileLog' type='RollingFileAppender' />
    <appender name='ApprendaAppender' type='log4net.Apprenda.ApprendaBufferingAppender, log4net.Apprenda'/>
</log4net>").ToString());
        }
    }
}
