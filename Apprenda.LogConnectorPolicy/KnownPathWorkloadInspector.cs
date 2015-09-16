namespace Apprenda.LogConnectorPolicy
{
    using System.Linq;

    using Apprenda.API.Extension.Bootstrapping;
    using Apprenda.Integrations.Inspection;

    /// <summary>
    /// Workload Inspector which modifies a specified path without probing.
    /// </summary>
    public class KnownPathWorkloadInspector : IWorkloadInspector
    {
        /// <summary>
        /// The path provided.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownPathWorkloadInspector"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public KnownPathWorkloadInspector(string path)
        {
            this._path = path;
        }

        /// <summary>
        /// Apply the 
        /// </summary>
        /// <returns>
        /// The <see cref="BootstrappingResult"/>.
        /// </returns>
        public BootstrappingResult Execute()
        {
            var worker = new Log4NetConfigurationUpdateService(_path);
            var results = worker.Update().ToArray();

            return results.Any() ? 
                BootstrappingResult.Failure(results) : 
                BootstrappingResult.Success();
        }
    }
}
