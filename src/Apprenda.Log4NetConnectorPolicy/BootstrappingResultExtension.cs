// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrappingResultExtension.cs" company="Apprenda, Inc.">
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
        /// The enumerable collection of error messages which 
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