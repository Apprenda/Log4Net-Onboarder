using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Apprenda.Log4NetConnectorPolicy.WorkloadUpdate
{
    public static class ConfigRuntimeBindingRedirectWorker
    {
        const string AssemblyNamespace = "urn:schemas-microsoft-com:asm.v1";

        public static void ModifyConfigurationElement(XElement configuration, BindingRedirectSettings _settings, ICollection<string> messageAccumulator)
        {
            if (_settings.OldVersion == _settings.NewVersion)
            {
                return;
            }

            var runtimeElement = GetOrCreateRuntimeElement(configuration);

            var assemblyBindingElement = GetOrCreateAssemblyBindngElement(runtimeElement, _settings, messageAccumulator);

            var elements = assemblyBindingElement.Elements(GetQualifiedName("dependentAssembly")).ToArray();
            var redirectsMatchingLibrary = elements
                .Where(da => 
                    da.Elements(GetQualifiedName("assemblyIdentity"))
                        .Any(ai => AssemblyIdMatchesSettings(ai, _settings))).ToArray();

            var shouldAddRedirect = true;
            if (redirectsMatchingLibrary.Any())
            {
                foreach (var dependentAssembly in redirectsMatchingLibrary)
                {
                    var assemblyIdentity = dependentAssembly.Element(GetQualifiedName("assemblyIdentity"));
                    if (assemblyIdentity == null)
                    {
                        break;
                    }
                    var assemblyPublicKeyAttribute = assemblyIdentity.Attribute("publicKeyToken");
                    if (assemblyPublicKeyAttribute == null && _settings.PublicKeyToken != null)
                    {
                        assemblyIdentity.Add(new XAttribute("publicKeyToken", _settings.PublicKeyToken));
                    }

                    var bindingRedirectElement = dependentAssembly.Element(GetQualifiedName("bindingRedirect"));
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
                        shouldAddRedirect = false;
                    }
                    else
                    {
                        var newVersion = new Version(newVersionAttribute.Value);
                        if (newVersion <= newSettingVersion)
                        {
                            newVersionAttribute.SetValue(newSettingVersion.ToString());
                            shouldAddRedirect = false;
                        }
                    }

                    var oldVersionAttribute = bindingRedirectElement.Attribute("oldVersion");
                    if (oldVersionAttribute == null)
                    {
                        oldVersionAttribute = new XAttribute("oldVersion", _settings.OldVersion);
                        bindingRedirectElement.Add(oldVersionAttribute);
                        shouldAddRedirect = false;
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
                                shouldAddRedirect = false;
                            }
                        }
                        else
                        {
                            var elementVersion = new Version(value);
                            if (elementVersion != settingsVersion)
                            {
                                var versionVector = new[] {elementVersion, settingsVersion};
                                
                                oldVersionAttribute.SetValue(string.Format("{0}-{1}", versionVector.Min(), versionVector.Max()));
                            }
                            shouldAddRedirect = false;
                        }
                    }

                }
            }

            if (shouldAddRedirect)
            {
                var dependentAssembly = new XElement(GetQualifiedName("dependentAssembly"));
                var assemblyIdentity = new XElement(GetQualifiedName("assemblyIdentity"), new XAttribute("name", _settings.AssemblyName),
                    new XAttribute("culture", _settings.Culture));
                if (_settings.PublicKeyToken != null)
                {
                    assemblyIdentity.Add(new XAttribute("publicKeyToken", _settings.PublicKeyToken));
                }

                dependentAssembly.Add(assemblyIdentity);
                var bindingNode = new XElement(GetQualifiedName("bindingRedirect"), new XAttribute("oldVersion", _settings.OldVersion),
                    new XAttribute("newVersion", _settings.NewVersion));
                dependentAssembly.Add(bindingNode);

                assemblyBindingElement.Add(dependentAssembly);
            }
        }
        public static XName GetQualifiedName(string name)
        {
            return XName.Get(name, AssemblyNamespace);
        }

        private static XElement GetOrCreateAssemblyBindngElement(XElement runtimeElement, BindingRedirectSettings _settings, ICollection<string>_messages)
        {
            XNamespace ns = AssemblyNamespace;

            var assemblyBindingElement = runtimeElement.Element(GetQualifiedName("assemblyBinding"));
            if (assemblyBindingElement == null)
            {
                assemblyBindingElement = new XElement(GetQualifiedName("assemblyBinding"), new XAttribute("xmlns", AssemblyNamespace));
                runtimeElement.Add(assemblyBindingElement);
            }

            var nsAttribute = assemblyBindingElement.Attributes("xmlns").FirstOrDefault();
            if (nsAttribute == null)
            {
                nsAttribute = new XAttribute("xmlns", AssemblyNamespace);
                assemblyBindingElement.Add(nsAttribute);
            }
            else if (!nsAttribute.Value.Equals(AssemblyNamespace))
            {
                if (_settings.CorrectNamespace)
                {
                    assemblyBindingElement.Remove();
                    var newAssemblyBindingElement = new XElement(ns + "assemblyBinding", new XAttribute("xmlns", AssemblyNamespace));
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

        private static bool AssemblyIdMatchesSettings(XElement ai, BindingRedirectSettings _settings)
        {
                return _settings.AssemblyName.Equals(ai.Attribute("name")?.Value) &&
                       _settings.Culture.Equals(ai.Attribute("culture")?.Value) &&
                       _settings.PublicKeyToken.Equals(ai.Attribute("publicKeyToken")?.Value);
        }

        private static XElement GetOrCreateRuntimeElement(XElement parentElement)
        {
            var runtimeElement = parentElement.XPathSelectElement("//runtime");
            if (runtimeElement == null)
            {
                runtimeElement = new XElement("runtime");
                parentElement.Add(runtimeElement);
            }
            return runtimeElement;
        }

    }
}
