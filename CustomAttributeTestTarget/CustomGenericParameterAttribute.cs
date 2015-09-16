namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom generic parameter attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class CustomGenericParameterAttribute : CustomTestAttribute
    {
    }
}