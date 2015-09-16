namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom method attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomMethodAttribute : CustomTestAttribute
    {
    }
}