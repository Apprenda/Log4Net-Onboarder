namespace Apprenda.LogConnectorPolicy
{
    using Apprenda.API.Extension.Bootstrapping;
    using Apprenda.Integrations.Inspection;

    using log4net.Config;

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
            return AssemblyExtensions.GetAssemblyAttributePropertyValue<XmlConfiguratorAttribute, string>(
                filePath,
                propertyName);
        }
    }
}