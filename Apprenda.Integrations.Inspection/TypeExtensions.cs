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
        /// Get the first custom attribute decorating <param name="target"/> or one of its ancestors.
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