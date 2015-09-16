namespace log4net.Apprenda
{
    using log4net.Appender;
    using log4net.Core;

    /// <summary>
    /// The Apprenda appender writes LoggingEvents to the Apprenda Platform Logging API
    /// </summary>
    public class ApprendaAppender : AppenderSkeleton
    {
        /// <summary>
        /// Appends a logging event to an event sink.
        /// </summary>
        /// <param name="loggingEvent">
        /// The logging event.
        /// </param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            var platformLogger =
                global::Apprenda.Services.Logging.LogManager.Instance().GetLogger(loggingEvent.LoggerName);
            platformLogger.Log(loggingEvent.RenderedMessage, loggingEvent.Level.ToApprendaLogLevel());
        }
    }
}