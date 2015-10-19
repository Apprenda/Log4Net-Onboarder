// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyExtensions.cs" company="Apprenda, Inc.">
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

    using Mono.Cecil;

    /// <summary>
    /// The assembly extensions.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Load an assembly definition and probe for the existence of a specific attribute type. If that attribute is present, return the value of the named argument
        /// (property) of the attribute. As this is intended to be infrequently executed code, no caching of the assembly definition is performed.
        /// </summary>
        /// <param name="assemblyPath">
        /// The path to the assembly to probe.
        /// </param>
        /// <param name="namedProperty">
        /// The name of the property (properly in CLR terms, the Name of the CustomAttributeNamedProperty) of the attribute type to be retrieved.
        /// </param>
        /// <typeparam name="TAttribute">
        /// The CLR type of the attribute to be probed for.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The CLR type of the property to be retrieved
        /// </typeparam>
        /// <returns>
        /// The <see cref="TValue"/> value of the named property.
        /// </returns>
        public static TValue GetAssemblyAttributePropertyValue<TAttribute, TValue>(
            string assemblyPath, 
            string namedProperty)
        {
            var attributeFullName = typeof(TAttribute).FullName;
            return GetAssemblyAttributePropertyValue<TValue>(assemblyPath, attributeFullName, namedProperty);
        }

        /// <summary>
        /// Load an assembly definition and probe for the existence of a specific attribute type by name. 
        /// If that attribute is present, return the value of the named argument (property) of that attribute.
        /// As this is intended for use in infrequently executed code, no caching of the assembly definition is performed.
        /// </summary>
        /// <param name="assemblyPath">
        /// The assembly path.
        /// </param>
        /// <param name="attributeFullName">
        /// The full name of the CLR type to be located and examined.
        /// </param>
        /// <param name="namedProperty">
        /// The named property.
        /// </param>
        /// <typeparam name="TValue">
        /// The type of the argument to be returned.
        /// </typeparam>
        /// <returns>
        /// The <see cref="TValue"/>.
        /// </returns>
        public static TValue GetAssemblyAttributePropertyValue<TValue>(
            string assemblyPath, 
            string attributeFullName,
            string namedProperty)
        {
            var defined = AssemblyDefinition.ReadAssembly(assemblyPath);
            var attribute =
                defined.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == attributeFullName);
            if (attribute == null)
            {
                return default(TValue);
            }

            var arguments =
                attribute.Properties.Where(
                    na => na.Name == namedProperty && na.Argument.Type.FullName == typeof(TValue).FullName).ToArray();
            if (!arguments.Any())
            {
                return default(TValue);
            }

            return (TValue)arguments[0].Argument.Value;
            
        }

        /// <summary>
        /// Check whether the probed assembly has a dependency on an assembly by name.
        /// </summary>
        /// <param name="probedAssembly">
        /// The path to the probed assembly.
        /// </param>
        /// <param name="assemblyName">
        /// The assembly name to check for dependence.
        /// </param>
        /// <returns>
        /// True if the assembly named is a dependency.
        /// </returns>
        public static bool HasDependencyOn(string probedAssembly, string assemblyName)
        {
            try
            {
                var defined = AssemblyDefinition.ReadAssembly(probedAssembly);

                return
                    defined.MainModule.AssemblyReferences.Any(
                        ar => ar.Name.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (BadImageFormatException)
            {
                // invalid as an assembly, therefore cannot trace dependency
                return false;
            }
        }
    }
}