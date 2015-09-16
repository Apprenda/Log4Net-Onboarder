namespace Apprenda.LogConnectorPolicy
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
            var properties = bootstrappingRequest.Properties.ToDictionary(k => k.Name);

            CustomProperty configFileProperty = null;

            if (properties.TryGetValue("log4net.ConfigurationFilePath", out configFileProperty))
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

                var results =
                    configFileProperty
                    .Values
                    .SelectMany(p => new Log4NetConfigurationUpdateService(p).Update())
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