namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom assembly attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CustomAssemblyAttribute : CustomTestAttribute
    {
    }
}
