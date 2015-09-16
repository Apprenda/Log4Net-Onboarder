namespace Apprenda.LogConnectorPolicy
{
    using Apprenda.API.Extension.Bootstrapping;
    using Apprenda.Integrations.Inspection;

    /// <summary>
    /// The ignored workload inspector is used to process workloads which an inspector connector wishes to ignore.
    /// </summary>
    public class IgnoredWorkloadInspector : IWorkloadInspector
    {
        /// <summary>
        /// The inspector worker function.
        /// </summary>
        /// <returns>
        /// The <see cref="BootstrappingResult"/> indicating success;
        /// </returns>
        public BootstrappingResult Execute()
        {
            return BootstrappingResult.Success();
        }
    }
}