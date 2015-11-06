// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkloadInspectorBase.cs" company="Apprenda, Inc.">
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
    using Apprenda.API.Extension.Bootstrapping;
    using Apprenda.Integrations.Inspection;

    /// <summary>
    /// The log4net workload inspector base.
    /// </summary>
    public abstract class WorkloadInspectorBase : IWorkloadInspector
    {
        /// <summary>
        /// When overridden in a WorkloadInspector, performs the work on the workload
        /// </summary>
        /// <returns>
        /// The <see cref="BootstrappingResult"/>.
        /// </returns>
        public abstract BootstrappingResult Execute();

        /// <summary>
        /// Retrieve the value of the named property on the XmlConfigurator attribute decorating the named assembly, if present.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetXmlConfiguratorProperty(string filePath, string propertyName)
        {
            return AssemblyExtensions.GetAssemblyAttributePropertyValue<string>(
                filePath, 
                "log4net.Config.XmlConfiguratorAttribute",
                propertyName);
        }
    }
}