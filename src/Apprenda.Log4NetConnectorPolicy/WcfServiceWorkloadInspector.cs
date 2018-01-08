// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WcfServiceWorkloadInspector.cs" company="Apprenda, Inc.">
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

using System;
using System.Collections.Generic;
using Apprenda.Log4NetConnectorPolicy.WorkloadUpdate;

namespace Apprenda.Log4NetConnectorPolicy
{
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Apprenda.API.Extension.Bootstrapping;
    using Apprenda.Integrations.Inspection;

    /// <summary>
    /// The WCF service workload inspector.
    /// </summary>
    public class WcfServiceWorkloadInspector : WorkloadInspectorBase
    {
        /// <summary>
        /// Bootstrapping context
        /// </summary>
        private readonly BootstrappingRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfServiceWorkloadInspector"/> class.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public WcfServiceWorkloadInspector(BootstrappingRequest request)
        {
            this._request = request;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <returns>
        /// The <see cref="BootstrappingResult"/>.
        /// </returns>
        public override BootstrappingResult Execute()
        {
            var messages = new List<string>();
            var assemblyPath = this._request.ComponentPath;
            Version appenderDependencyVersion;
            string appenderDepedencyPublicKey;
            if (!File.Exists(Path.Combine(assemblyPath, "log4net.dll")))
            {
                // no log4net requires no modifications.
                return BootstrappingResult.Success();
            }

            var potentialAssemblies =
                Directory.EnumerateFiles(assemblyPath, "*.dll")
                    .Where(p => AssemblyExtensions.HasDependencyOn(p, "log4net"))
                    .ToArray();

            if (potentialAssemblies.Any())
            {
                using (var assemblyStream =
                       Assembly.GetExecutingAssembly()
                           .GetManifestResourceStream("Apprenda.Log4NetConnectorPolicy.Resources.log4net.Apprenda.dll"))
                {
                    if (assemblyStream != null)
                    {
                        var appenderPath = Path.Combine(assemblyPath, "log4net.Apprenda.dll");
                        if (!File.Exists(appenderPath))
                        {
                            using (var appenderStream = new FileStream(appenderPath, FileMode.Create))
                            {
                                assemblyStream.CopyTo(appenderStream);
                            }
                        }

                        appenderDependencyVersion = AssemblyExtensions.GetDependencyVersion(appenderPath, "log4net");
                        appenderDepedencyPublicKey =
                            AssemblyExtensions.GetDependencyPublicKeyToken(appenderPath, "log4net");
                    }
                    else
                    {
                        return BootstrappingResult.Failure(new[] { "Failed to copy logging assembly to the output path." });
                    }
                }
                var oldVersion = appenderDependencyVersion.ToString();

                var projected = potentialAssemblies
                    .Select(pa => new {Path = pa, DepVersion = AssemblyExtensions.GetDependencyVersion(pa, "log4net")}).ToArray();

                var filtered = projected
                    .Where(proj => proj.DepVersion > appenderDependencyVersion).ToArray();

                var configFiles = filtered
                    .Select(pa => new { Path = pa.Path + ".config", pa.DepVersion })
                    .Where(pa => File.Exists(pa.Path))
                    .ToArray();

                var newVersion = configFiles.Max(cf => cf.DepVersion)?.ToString();

                foreach (var configDetail in configFiles)
                {
                    var updateService = new ConfigBindingRedirectUpdateService(configDetail.Path, new BindingRedirectSettings
                    {
                        AssemblyName = "log4net",
                        Culture = "neutral",
                        NewVersion = newVersion,
                        OldVersion = oldVersion,
                        PublicKeyToken = appenderDepedencyPublicKey
                    });
                    messages.AddRange(updateService.Update());

                }
            }
            if (!messages.Any())
            {
                var configFilePaths = potentialAssemblies.Select(
                    filePath =>
                    {
                        var configFileName = GetXmlConfiguratorProperty(filePath, "ConfigFile");
                        if (configFileName != null)
                        {
                            return Path.Combine(assemblyPath, configFileName);
                        }

                        var configExtension = GetXmlConfiguratorProperty(filePath, "ConfigFileExtension") ?? "config";
                        return filePath + (configExtension.StartsWith(".") ? string.Empty : ".") + configExtension;
                    });

                messages.AddRange(
                    configFilePaths.SelectMany(
                        configFilePath => new Log4NetConfigurationUpdateService(configFilePath).Update()));
            }

            return BootstrappingResultExtension.SuccessIfNoMessages(messages);
        }
    }
}