// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log4NetAppConfigUpdateService.cs" company="Apprenda, Inc.">
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
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;

    /// <summary>
    /// The log 4 net app config update service.
    /// </summary>
    public class Log4NetAppConfigUpdateService
    {
        /// <summary>
        /// Path to the document to modify.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// Error message accumulator.
        /// </summary>
        private List<string> _messages;

        /// <summary>
        /// The XDocument being modified.
        /// </summary>
        private XDocument _document;

        /// <summary>
        /// The app.config root configuration node.
        /// </summary>
        private XElement _configurationNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetAppConfigUpdateService"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If path is null or empty. 
        /// </exception>
        public Log4NetAppConfigUpdateService(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path to configuration file must be supplied.", "path");
            }

            _path = path;
        }

        /// <summary>
        /// Updates the XML Application configuration file.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> containing any error messages generated during the modification process.
        /// </returns>
        public IEnumerable<string> Update()
        {
            _messages = new List<string>();
            LoadDocument();
            var log4netElement = _document.XPathSelectElement("//log4net");
            if (log4netElement == null)
            {
                AddIgnoredConfigSection();
                log4netElement = this.AddLog4NetElement();
            }

            LogElementWorker.UpdateLoggingElement(log4netElement);

            SaveDocument();
            return _messages;
        }

        /// <summary>
        /// The add default logging element.
        /// </summary>
        /// <returns>
        /// The <see cref="XElement"/> which was added.
        /// </returns>
        private XElement AddLog4NetElement()
        {
            if (_messages.Any())
            {
                _messages.Add("Skipping adding Apprenda configuration log4net section due to previous messages.");
                return null;
            }

            var loggingElement = new XElement("log4net");

            _configurationNode.Add(loggingElement);

            return loggingElement;
        }

        /// <summary>
        /// Analyze the configuration file and add the log4net config section declaration before injecting a log4net configuration section, if needed.
        /// </summary>
        private void AddIgnoredConfigSection()
        {
            if (_messages.Any())
            {
                _messages.Add("Skipping configuration section update due to previous messages.");
                return;
            }

            _configurationNode = _document.Element("configuration");
            if (_configurationNode == null)
            {
                _messages.Add(
                    "No <configuration> element was found, which is required for Web.config and app.config. Something drastic seems to be wrong with the workload.");
                return;
            }

            var configurationSectionsElement = _document.XPathSelectElement("//configuration/configSections");
            if (configurationSectionsElement == null)
            {
                configurationSectionsElement = new XElement("configSections");
                _configurationNode.Add(configurationSectionsElement);
            }

            var logSectionConfigElement = configurationSectionsElement.XPathSelectElement("//section[@name='log4net']");
            if (logSectionConfigElement == null)
            {
                configurationSectionsElement.Add(
                    new XElement(
                        "section", 
                        new XAttribute("name", "log4net"), 
                        new XAttribute("type", "System.Configuration.IgnoreSectionHandler")));
            }
        }

        /// <summary>
        /// Attempt to load the configuration file
        /// </summary>
        private void LoadDocument()
        {
            try
            {
                _document = XDocument.Load(_path);
            }
            catch (Exception ex)
            {
                _messages.Add(ex.ToString());
            }
        }

        /// <summary>
        /// Save the configuration file.
        /// </summary>
        private void SaveDocument()
        {
            try
            {
                _document.Save(_path);
            }
            catch (Exception ex)
            {
                _messages.Add(ex.ToString());
            }
        }
    }
}