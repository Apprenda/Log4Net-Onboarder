namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom delegate attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Delegate)]
    public class CustomDelegateAttribute : CustomTestAttribute
    {
    }
}