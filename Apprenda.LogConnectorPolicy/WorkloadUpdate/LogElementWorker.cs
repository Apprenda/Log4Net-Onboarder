namespace Apprenda.LogConnectorPolicy
{
    using System.Xml.Linq;
    using System.Xml.XPath;

    /// <summary>
    /// The log element worker.
    /// </summary>
    public static class LogElementWorker
    {
        /// <summary>
        /// Update the content of the logging element.
        /// </summary>
        /// <param name="loggingElement">
        /// The logging element being modified.
        /// </param>
        public static void UpdateLoggingElement(XElement loggingElement)
        {
            var rootElement = loggingElement.Element("root");
            if (rootElement == null)
            {
                rootElement = new XElement("root", new XElement("level", new XAttribute("value", "DEBUG")));
            }
            else
            {
                var levelElt = rootElement.Element("level");
                if (levelElt == null)
                {
                    rootElement.Add(new XElement("level", new XAttribute("value", "DEBUG")));
                }
                else
                {
                    levelElt.ReplaceAttributes(new XAttribute("value", "DEBUG"));
                }
            }

            var apprendaAppender = loggingElement.XPathSelectElement("//appender[@type='log4net.ApprendaBufferingAppender']");
            if (apprendaAppender == null)
            {
                loggingElement.Add(
                    new XElement(
                        "appender", 
                        new XAttribute("name", "ApprendaAppender"), 
                        new XAttribute("type", "log4net.Apprenda.ApprendaBufferingAppender")));
                rootElement.Add("appender-ref", new XAttribute("ref", "ApprendaAppender"));
            }
            else
            {
                var apprendaAppenderName = apprendaAppender.Attribute("name").Value;
                var appenderRef =
                    rootElement.XPathSelectElement(
                        "//appender-ref[@name='" + apprendaAppenderName + "'");
                if (appenderRef == null)
                {
                    rootElement.Add(new XElement("appender-ref", new XAttribute("ref", apprendaAppenderName)));
                }
            }
        }
    }
}
