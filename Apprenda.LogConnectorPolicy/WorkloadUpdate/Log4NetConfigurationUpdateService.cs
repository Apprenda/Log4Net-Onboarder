// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetConfigurationUpdateService.cs" company="Apprenda, Inc.">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Xml.XPath;

    /// <summary>
    /// The log4net standalone configuration file update service.
    /// </summary>
    public class Log4NetConfigurationUpdateService
    {
        /// <summary>
        /// The Path to the XML configuration file to modify
        /// </summary>
        private readonly string _configFilePath;

        /// <summary>
        /// The configuration document being modified.
        /// </summary>
        private XDocument _configurationDocument;

        /// <summary>
        /// Message accumulator for the configuration update.
        /// </summary>
        private List<string> _messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetConfigurationUpdateService"/> class.
        /// </summary>
        /// <param name="configFilePath">
        /// The config file path.
        /// </param>
        public Log4NetConfigurationUpdateService(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        /// <summary>
        /// Perform the configuration file update within the workload.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> of error messages.
        /// </returns>
        public IEnumerable<string> Update()
        {
            _messages = new List<string>();

            if (!File.Exists(_configFilePath))
            {
                InjectDefaultConfiguration();
            }
            else
            {
                LoadConfiguration();
            }

            UpdateConfigurationDocument();
            WriteConfigurationDocument();

            return _messages;
        }

        /// <summary>
        /// Inject default configuration.
        /// </summary>
        private void InjectDefaultConfiguration()
        {
            _configurationDocument = new XDocument();
        }

        /// <summary>
        /// Load the configuration document.
        /// </summary>
        private void LoadConfiguration()
        {
            try
            {
                _configurationDocument = XDocument.Load(_configFilePath);
            }
            catch (Exception ex)
            {
                _messages.Add(ex.ToString());
            }
        }

        /// <summary>
        /// Update main worker.
        /// </summary>
        private void UpdateConfigurationDocument()
        {
            if (_messages.Any())
            {
                _messages.Add("Skipping UpdateConfigurationDocument due to previous messages.");
                return;
            }

            var loggingElement = _configurationDocument.XPathSelectElement("//log4net");
            if (loggingElement == null)
            {
                loggingElement = new XElement("log4net");
                _configurationDocument.Add(loggingElement);
            }

            LogElementWorker.UpdateLoggingElement(loggingElement);
        }

        /// <summary>
        /// Write the configuration document.
        /// </summary>
        private void WriteConfigurationDocument()
        {
            try
            {
                _configurationDocument.Save(_configFilePath);
            }
            catch (Exception ex)
            {
                _messages.Add(ex.ToString());
            }
        }
    }
}