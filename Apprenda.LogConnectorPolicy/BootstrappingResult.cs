namespace Apprenda.LogConnectorPolicy
{
    using System.Collections.Generic;
    using System.Linq;

    using Apprenda.API.Extension.Bootstrapping;

    /// <summary>
    /// The bootstrapping result extensions.
    /// </summary>
    public class BootstrappingResultExtension
    {
        /// <summary>
        /// Roll up multiple bootstrapping results, success if no error messages 
        /// </summary>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <returns>
        /// The <see cref="BootstrappingResult"/>.
        /// </returns>
        public static BootstrappingResult Consolidate(IEnumerable<BootstrappingResult> results)
        {
            var realized = results as BootstrappingResult[] ?? results.ToArray();
            var rolledUp = SuccessIfNoMessages(realized.SelectMany(r => r.Errors));
            rolledUp.Succeeded = realized.All(r => r.Succeeded);
            return rolledUp;
        }

        /// <summary>
        /// Use an enumerable collection of error messages to determine if a BootstrappingResult of Success or Failure should be returned.
        /// </summary>
        /// <param name="messages">
        ///     The enumerable collection of error messages which 
        /// </param>
        /// <returns>
        /// The <see cref="BootstrappingResult"/>.
        /// </returns>
        public static BootstrappingResult SuccessIfNoMessages(IEnumerable<string> messages)
        {
            var realized = messages.ToArray();

            return realized.Any() ? BootstrappingResult.Failure(realized) : BootstrappingResult.Success();
        }
    }
}