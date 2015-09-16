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
        public static TValue GetAssemblyAttributePropertyValue<TAttribute, TValue>(string assemblyPath, string namedProperty)
        {
            var defined = AssemblyDefinition.ReadAssembly(assemblyPath);
            var attribute = defined.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.FullName == typeof(TAttribute).FullName);
            if (attribute == null)
            {
                return default(TValue);
            }

            var arguments = attribute.Properties.Where(na => na.Name == namedProperty && na.Argument.Type.FullName == typeof(TValue).FullName).ToArray();
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
            var defined = AssemblyDefinition.ReadAssembly(probedAssembly);
            return
                defined.MainModule.AssemblyReferences
                .Any(ar => ar.Name.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
