namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom field attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomFieldAttribute : CustomTestAttribute
    {
    }
}