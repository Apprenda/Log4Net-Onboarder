namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom property attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomPropertyAttribute : CustomTestAttribute
    {
    }
}