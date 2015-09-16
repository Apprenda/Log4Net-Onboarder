namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// A custom attribute to attach to classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomClassAttribute : CustomTestAttribute
    {
    }
}