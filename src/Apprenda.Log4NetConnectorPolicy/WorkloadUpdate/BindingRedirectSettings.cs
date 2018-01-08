namespace Apprenda.Log4NetConnectorPolicy.WorkloadUpdate
{
    public class BindingRedirectSettings
    {
        /// <summary>
        /// The assembly's short name to be redirected
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// The assembly's public key token for strong-named assemblies
        /// </summary>
        public string PublicKeyToken { get; set; }

        /// <summary>
        /// The culture of the target assembly or assemblies to redirect
        /// </summary>
        public string Culture { get; set; } = "neutral";

        /// <summary>
        /// The version that the injected appender depends on, to allow workloads to update faster than platform operators update the dependency.
        /// </summary>
        public string OldVersion { get; set; }

        /// <summary>
        /// The version that the workload depends on, so all old versions of dependencies should be redirected to the workload version.
        /// Because this is being deployed as part of the log4net support bootstrap policy, the threat of introducing faults through updated versions of log4net is very low.
        /// </summary>
        public string NewVersion { get; set; }
        /// <summary>
        /// If set true, the worker should update the namespace of the asemblyBinding element if it does not match 'urn:schemas-microsoft-com:asm.v1'
        /// Default value: true
        /// </summary>
        public bool CorrectNamespace { get; set; } = true;
    }
}