// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DotNetLog4NetConnector.cs" company="Apprenda, Inc.">
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Apprenda.Log4NetConnectorPolicy
{
    using System.Linq;

    using Apprenda.API.Extension.Bootstrapping;
    using Apprenda.API.Extension.CustomProperties;
    using Apprenda.Integrations.Inspection;

    /// <summary>
    /// Bootstrap Policy to connect .Net workloads to the Apprenda logging facility
    /// </summary>
    public class DotNetLog4NetConnector : BootstrapperBase
    {
        /// <summary>
        /// Component types supported for this bootstrap policy.
        /// Note that ComponentType.WindowsService does not have a mechanism to connect to the Apprenda API at this time, 
        /// so we do not allow connection to that component type.
        /// </summary>
        private readonly ComponentType[] _allowComponentTypes =
            {
                ComponentType.AspNet, ComponentType.PublicAspNet, 
                ComponentType.WcfService
            };

        /// <summary>
        /// The bootstrap.
        /// </summary>
        /// <param name="bootstrappingRequest">
        /// The bootstrapping bootstrappingRequest.
        /// </param>
        /// <returns>
        /// The <see cref="BootstrappingResult"/>.
        /// </returns>
        public override BootstrappingResult Bootstrap(BootstrappingRequest bootstrappingRequest)
        {
            IWorkloadInspector inspector;

            foreach (var configFileProperty in
                    bootstrappingRequest.Properties.Where(cp => cp.Name.Equals("log4net.ConfigurationFilePath")))
            {
                if (!this._allowComponentTypes.Contains(bootstrappingRequest.ComponentType))
                {
                    return
                        BootstrappingResult.Failure(
                            new[]
                                {
                                    "Cannot apply explicit log4net configuration to component type "
                                    + bootstrappingRequest.ComponentType + "."
                                });
                }
                if (!configFileProperty.Values.Any())
                {
                    continue;
                }

                var results =
                    configFileProperty.Values.SelectMany(p => new Log4NetConfigurationUpdateService(p).Update())
                        .ToArray();
                return results.Any() ? BootstrappingResult.Failure(results) : BootstrappingResult.Success();
            }

            switch (bootstrappingRequest.ComponentType)
            {
                case ComponentType.PublicAspNet:
                case ComponentType.AspNet:
                    inspector = new UserInterfaceWorkloadInspector(bootstrappingRequest);
                    break;
                case ComponentType.WcfService:
                    inspector = new WcfServiceWorkloadInspector(bootstrappingRequest);
                    break;
                default:
                    inspector = new IgnoredWorkloadInspector();
                    break;
            }

            return inspector.Execute();
        }
    }
}