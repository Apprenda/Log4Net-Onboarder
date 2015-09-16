namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom event attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event)]
    public class CustomEventAttribute : CustomTestAttribute
    {
    }
}
