namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom constructor attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class CustomConstructorAttribute : CustomTestAttribute
    {
    }
}