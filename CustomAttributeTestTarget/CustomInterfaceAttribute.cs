namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom interface attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class CustomInterfaceAttribute : CustomTestAttribute
    {
    }
}