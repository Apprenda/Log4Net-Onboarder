namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// The custom attribute for <c>System.Enum</c>
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class CustomEnumAttribute : CustomTestAttribute
    {
    }
}