namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom return value attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue)]
    public class CustomReturnValueAttribute : CustomTestAttribute
    {
    }
}