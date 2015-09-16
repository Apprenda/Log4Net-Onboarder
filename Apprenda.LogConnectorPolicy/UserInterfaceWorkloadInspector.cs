namespace Apprenda.LogConnectorPolicy
{
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Apprenda.API.Extension.Bootstrapping;
    using Apprenda.Integrations.Inspection;

    /// <summary>
    /// The IWorkloadInspector to probe for the presence of the log4net assembly in the workload and determine 
    /// whether the log4net configuration is loaded from the app.config (web.config) or a separate log4net XML document.
    /// When found, the log4net configuration is updated to replace the existing appender configuration with the Apprenda appender
    /// and the Apprenda appender assembly is added to the workload.
    /// </summary>
    public class UserInterfaceWorkloadInspector : WorkloadInspectorBase
    {
        /// <summary>
        /// The request being inspected.
        /// </summary>
        private readonly BootstrappingRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInterfaceWorkloadInspector"/> class.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public UserInterfaceWorkloadInspector(BootstrappingRequest request)
        {
            this._request = request;
        }

        /// <summary>
        /// Performs the work of inspecting a workload for the presence of log4net configuration to be modified,
        /// and making those modifications if present.
        /// </summary>
        /// <returns>
        /// The <see cref="BootstrappingResult"/>.
        /// </returns>
        public override BootstrappingResult Execute()
        {
            var uiComponentResults =
                Directory.EnumerateDirectories(this._request.ComponentPath)
                    .Where(d => File.Exists(Path.Combine(d, "bin", "log4net.dll")))
                    .Select(this.ProbeUiComponentForAssemblies)
                    .ToArray();

            var result = BootstrappingResultExtension.Consolidate(uiComponentResults);
            return result;
        }

        /// <summary>
        /// Probe each UI component and attach our appender.
        /// </summary>
        /// <param name="uiPath">
        /// The path to this user interface component.
        /// </param>
        /// <returns>
        /// A <see cref="BootstrappingResult"/> which summarizes the outcome of the workload. All component UIs which return BootstrappingResult.Failure will have their 
        /// error collections rolled up into the single returned BootstrappingResult.
        /// </returns>
        private BootstrappingResult ProbeUiComponentForAssemblies(string uiPath)
        {
            var binaryPath = Path.Combine(uiPath, "bin");
            if (!File.Exists(Path.Combine(binaryPath, "log4net.dll")))
            {
                // if log4net is not present in the component, this bootstrapper succeeds by not modifying the workload.
                return BootstrappingResult.Success();
            }

            var assemblyStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Apprenda.Log4NetConnectorPolicy.Resources.log4net.Apprenda.dll");
            if (assemblyStream != null)
            {
                var appenderPath = Path.Combine(binaryPath, "log4net.Apprenda.dll");
                if (!File.Exists(appenderPath))
                {
                    assemblyStream.CopyTo(new FileStream(appenderPath, FileMode.Create));
                }
            }
            else
            {
                return BootstrappingResult.Failure(new[] { "Failed to copy logging assembly to the output path." });
            }

            var potentialAssemblies =
                Directory.EnumerateFiles(binaryPath, "*.dll")
                    .Where(p => AssemblyExtensions.HasDependencyOn(p, "log4net"));

            var configFilePaths = potentialAssemblies.Select(
                filePath =>
                    {
                        var configFileName = GetXmlConfiguratorProperty(filePath, "ConfigFile");
                        if (configFileName != null)
                        {
                            return Path.Combine(uiPath, configFileName);
                        }

                        var configExtension = GetXmlConfiguratorProperty(filePath, "ConfigFileExtension") ?? "config";
                        return filePath + (configExtension.StartsWith(".") ? "." : string.Empty) + configExtension;
                    });

            var messages =
                configFilePaths.SelectMany(
                    configFilePath => new Log4NetConfigurationUpdateService(configFilePath).Update());

            var webConfigUpdateMessages = new Log4NetAppConfigUpdateService(Path.Combine(uiPath, "Web.config")).Update();
            
            return BootstrappingResultExtension.SuccessIfNoMessages(messages.Union(webConfigUpdateMessages));
        }
    }
}