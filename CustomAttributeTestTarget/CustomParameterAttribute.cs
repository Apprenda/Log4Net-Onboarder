namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom parameter attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class CustomParameterAttribute : CustomTestAttribute
    {
    }
}