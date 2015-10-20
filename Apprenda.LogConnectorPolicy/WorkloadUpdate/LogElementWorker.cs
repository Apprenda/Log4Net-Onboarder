// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogElementWorker.cs" company="Apprenda, Inc.">
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
namespace Apprenda.Log4NetConnectorPolicy
{
    using System.Xml.Linq;
    using System.Xml.XPath;

    /// <summary>
    /// The log element worker.
    /// </summary>
    public static class LogElementWorker
    {
        /// <summary>
        /// Update the content of the logging element.
        /// </summary>
        /// <param name="loggingElement">
        /// The logging element being modified.
        /// </param>
        public static void UpdateLoggingElement(XElement loggingElement)
        {
            var rootElement = loggingElement.Element("root");
            if (rootElement == null)
            {
                rootElement = new XElement("root", new XElement("level", new XAttribute("value", "DEBUG")));
                loggingElement.Add(rootElement);
            }
            else
            {
                var levelElt = rootElement.Element("level");
                if (levelElt == null)
                {
                    rootElement.Add(new XElement("level", new XAttribute("value", "DEBUG")));
                }
                else
                {
                    levelElt.ReplaceAttributes(new XAttribute("value", "DEBUG"));
                }
            }

            var apprendaAppender =
                loggingElement.XPathSelectElement("//appender[@type='log4net.ApprendaBufferingAppender']");
            if (apprendaAppender == null)
            {
                loggingElement.Add(
                    new XElement(
                        "appender", 
                        new XAttribute("name", "ApprendaAppender"), 
                        new XAttribute("type", "log4net.Apprenda.ApprendaAppender, log4net.Apprenda")));
                rootElement.Add(new XElement("appender-ref", new XAttribute("ref", "ApprendaAppender")));
            }
            else
            {
                var apprendaAppenderName = apprendaAppender.Attribute("name").Value;
                var appenderRef = rootElement.XPathSelectElement("//appender-ref[@name='" + apprendaAppenderName + "'");
                if (appenderRef == null)
                {
                    rootElement.Add(new XElement("appender-ref", new XAttribute("ref", apprendaAppenderName)));
                }
            }
        }
    }
}