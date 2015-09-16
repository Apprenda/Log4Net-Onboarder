namespace Apprenda.Integrations.Inspection
{
    using Apprenda.API.Extension.Bootstrapping;

    /// <summary>
    /// The WorkloadInspector interface.
    /// </summary>
    public interface IWorkloadInspector
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <returns>
        /// The <see cref="BootstrappingResult"/>.
        /// </returns>
        BootstrappingResult Execute();
    }
}
