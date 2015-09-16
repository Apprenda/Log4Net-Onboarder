namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom module target.
    /// </summary>
    [AttributeUsage(AttributeTargets.Module)]
    public class CustomModuleTarget : CustomTestAttribute
    {
    }
}