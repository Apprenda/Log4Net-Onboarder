using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Apprenda.Log4NetConnectorPolicy.WorkloadUpdate
{
    public class ConfigBindingRedirectUpdateService
    {
        private readonly string _path;
        private readonly BindingRedirectSettings _settings;
        private XDocument _document;
        private List<string> _messages;
        private XElement _configurationElement;

        /// <summary>
        /// When provided the path to a .Net app.config (including web.config),
        /// this service will attempt to probe for a binding redirect stanza,
        /// update it to add or update a redirection for a specified assembly 
        /// to an specified version.
        /// </summary>
        public ConfigBindingRedirectUpdateService(string path, BindingRedirectSettings settings)
        {
            _path = path;
            _settings = settings;
        }

        public IEnumerable<string> Update()
        {
            _messages = new List<string>();

            if (!File.Exists(_path))
            {
                _messages.Add(string.Format("No configuration file was found at {0}", _path));
                return _messages;
            }
            LoadDocument();

            _configurationElement = _document.XPathSelectElement("//configuration");
            if (_configurationElement == null)
            {
                _messages.Add("Could not find root configuration element in application configuration file.");
                return _messages;
            }

            ConfigRuntimeBindingRedirectWorker.ModifyConfigurationElement(_configurationElement, _settings, _messages);
            if (!_messages.Any())
            {
                SaveDocument();
            }

            return _messages;
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