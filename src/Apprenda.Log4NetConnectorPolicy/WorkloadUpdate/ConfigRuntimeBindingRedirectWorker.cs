using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Apprenda.Log4NetConnectorPolicy.WorkloadUpdate
{
    public static class ConfigRuntimeBindingRedirectWorker
    {
        public static void ModifyConfigurationElement(XElement configuration, BindingRedirectSettings _settings, ICollection<string> messageAccumulator)
        {
            if (_settings.OldVersion == _settings.NewVersion)
            {
                return;
            }

            var runtimeElement = GetOrCreateRuntimeElement(configuration);

            var assemblyBindingElement = GetOrCreateAssemblyBindngElement(runtimeElement, _settings, messageAccumulator);
            
            var identityTarget =
                string.Format(
                    "//dependentAssembly[assemblyIdentity/@name='{0}' and @publicKeyToken='{1}'  and culture='{2}']",
                    _settings.AssemblyName,
                    _settings.PublicKeyToken,
                    _settings.Culture);

            var redirectsMatchingLibrary = assemblyBindingElement.XPathSelectElements(identityTarget).ToArray();

            var modifiedElements = 0;
            if (redirectsMatchingLibrary.Any())
            {
                foreach (var dependentAssembly in redirectsMatchingLibrary)
                {
                    var hasModification = false;
                    var assemblyIdentity = dependentAssembly.XPathSelectElement("/assemblyIdentity");
                    if (assemblyIdentity == null)
                    {
                        break;
                    }
                    var assemblyPublicKeyAttribute = assemblyIdentity.Attribute("publicKeyToken");
                    if (assemblyPublicKeyAttribute == null && _settings.PublicKeyToken != null)
                    {
                        assemblyIdentity.Add(new XAttribute("publicKeyToken", _settings.PublicKeyToken));
                    }

                    var bindingRedirectElement = dependentAssembly.XPathSelectElement("/bindingRedirect");
                    if (bindingRedirectElement == null)
                    {
                        break;

                    }

                    var newSettingVersion = new Version(_settings.NewVersion);

                    var newVersionAttribute = bindingRedirectElement.Attribute("newVersion");
                    if (newVersionAttribute == null)
                    {
                        newVersionAttribute = new XAttribute("newVersion", _settings.NewVersion);
                        bindingRedirectElement.Add(newVersionAttribute);
                        hasModification = true;
                    }
                    else
                    {
                        var newVersion = new Version(newVersionAttribute.Value);
                        if (newVersion < newSettingVersion)
                        {
                            newVersionAttribute.SetValue(newSettingVersion.ToString());
                            hasModification = true;
                        }
                    }

                    var oldVersionAttribute = bindingRedirectElement.Attribute("oldVersion");
                    if (oldVersionAttribute == null)
                    {
                        oldVersionAttribute = new XAttribute("oldVersion", _settings.OldVersion);
                        bindingRedirectElement.Add(oldVersionAttribute);
                        hasModification = true;
                    }
                    else
                    {
                        var settingsVersion = new Version(_settings.OldVersion);

                        var value = oldVersionAttribute.Value;
                        if (value.Contains("-"))
                        {
                            var range = value.Split('-');

                            var minVersion = new Version(range[0]);
                            var maxVersion = new Version(range[1]);
                            if (minVersion < settingsVersion && settingsVersion < maxVersion)
                            {
                                // redirect already points toward the right thing....
                            }
                            else
                            {
                                if (settingsVersion < minVersion)
                                {
                                    minVersion = settingsVersion;
                                }

                                if (settingsVersion > maxVersion)
                                {
                                    maxVersion = settingsVersion;
                                }

                                oldVersionAttribute.SetValue(string.Format("{0}-{1}", minVersion, maxVersion));
                                hasModification = true;
                            }

                        }
                    }

                    if (hasModification)
                    {
                        modifiedElements++;
                    }
                }
            }

            if (modifiedElements == 0)
            {
                var dependentAssembly = new XElement("dependentAssembly");
                var assemblyIdentity = new XElement("assemblyIdentity", new XAttribute("name", _settings.AssemblyName),
                    new XAttribute("culture", _settings.Culture));
                if (_settings.PublicKeyToken != null)
                {
                    assemblyIdentity.Add(new XAttribute("publicKeyToken", _settings.PublicKeyToken));
                }

                dependentAssembly.Add(assemblyIdentity);
                var bindingNode = new XElement("bindingRedirect", new XAttribute("oldVersion", _settings.OldVersion),
                    new XAttribute("newVersion", _settings.NewVersion));
                dependentAssembly.Add(bindingNode);

                assemblyBindingElement.Add(dependentAssembly);
            }
        }


        private static XElement GetOrCreateAssemblyBindngElement(XElement runtimeElement, BindingRedirectSettings _settings, ICollection<string>_messages)
        {
            const string xmlNs = "urn:schemas-microsoft-com:asm.v1";
            XNamespace ns = xmlNs;

            var assemblyBindingElement = runtimeElement.XPathSelectElement("/assemblyBinding");
            if (assemblyBindingElement == null)
            {
                assemblyBindingElement = new XElement(ns + "assemblyBinding", new XAttribute("xmlns", xmlNs));
                runtimeElement.Add(assemblyBindingElement);
            }

            var nsAttribute = assemblyBindingElement.Attributes("xmlns").FirstOrDefault();
            if (nsAttribute == null)
            {
                nsAttribute = new XAttribute("xmlns", xmlNs);
                assemblyBindingElement.Add(nsAttribute);
            }
            else if (!nsAttribute.Value.Equals(xmlNs))
            {
                if (_settings.CorrectNamespace)
                {
                    assemblyBindingElement.Remove();
                    var newAssemblyBindingElement = new XElement(ns + "assemblyBinding", new XAttribute("xmlns", xmlNs));
                    newAssemblyBindingElement.Add(assemblyBindingElement.Attributes(), assemblyBindingElement.Descendants());
                    runtimeElement.Add(newAssemblyBindingElement);
                }
                else
                {
                    _messages.Add(
                        "Found an assemblyBinding stanza in application configuration which is missing the correct xmlns attribute; this will cause unpredictable behavior at runtime.");
                    return assemblyBindingElement;
                }
            }
            return assemblyBindingElement;
        }

        private static XElement GetOrCreateRuntimeElement(XElement parentElement)
        {
            var runtimeElement = parentElement.XPathSelectElement("/runtime");
            if (runtimeElement == null)
            {
                runtimeElement = new XElement("runtime");
                parentElement.Add(runtimeElement);
            }
            return runtimeElement;
        }

    }
}
