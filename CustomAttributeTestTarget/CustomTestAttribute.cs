namespace CustomAttributeTestTarget
{
    using System;

    /// <summary>
    /// Parent of Apprenda
    /// </summary>
    public abstract class CustomTestAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
    }
}