// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Apprenda, Inc.">
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
namespace Apprenda.Integrations.Inspection
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// The has target attribute.
        /// </summary>
        /// <param name="target">
        /// The target to check for custom attributes.
        /// </param>
        /// <param name="probeAncestors">
        /// If true, ascend the inheritance tree to probe ancestors for this attribute.
        /// </param>
        /// <typeparam name="T">
        /// The <see cref="System.Type"/> of the target attribute to search for on the target type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider target, bool probeAncestors = false)
            where T : Attribute
        {
            if (target == null)
            {
                return false;
            }

            return target.GetCustomAttribute<T>(probeAncestors) != null;
        }

        /// <summary>
        /// Get the first custom attribute decorating  or one of its ancestors.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="probeAncestors">
        /// If true, ascend the inheritance tree to probe for this attribute.
        /// </param>
        /// <typeparam name="TAttribute">
        /// The <see cref="System.Type"/> of the custom attribute.
        /// </typeparam>
        /// <returns>
        /// The <see cref="TAttribute"/> custom attribute.
        /// </returns>
        public static TAttribute GetCustomAttribute<TAttribute>(
            this ICustomAttributeProvider target, 
            bool probeAncestors = false) where TAttribute : Attribute
        {
            if (target == null)
            {
                return null;
            }

            return (TAttribute)target.GetCustomAttributes(typeof(TAttribute), probeAncestors).FirstOrDefault();
        }
    }
}