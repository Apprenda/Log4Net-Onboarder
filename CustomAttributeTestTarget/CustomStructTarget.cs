namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom struct target.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    public class CustomStructTarget : CustomTestAttribute
    {
    }
}