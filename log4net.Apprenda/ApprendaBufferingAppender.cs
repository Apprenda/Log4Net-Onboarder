namespace log4net.Apprenda
{
    using log4net.Appender;
    using log4net.Core;

    /// <summary>
    /// The Apprenda buffering appender.
    /// </summary>
    public class ApprendaBufferingAppender : BufferingAppenderSkeleton
    {
        /// <summary>
        /// Sends all buffered events to the appender.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        protected override void SendBuffer(LoggingEvent[] events)
        {
            foreach (var evt in events)
            {
                SendEvent(evt);
            }
        }

        /// <summary>
        /// Sends a LoggingEvent to the Apprenda Platform Logging API
        /// </summary>
        /// <param name="loggingEvent">
        /// The logging event.
        /// </param>
        private static void SendEvent(LoggingEvent loggingEvent)
        {
            var platformLogger = global::Apprenda.Services.Logging.LogManager.Instance().GetLogger(loggingEvent.LoggerName);
            platformLogger.Log(loggingEvent.RenderedMessage, loggingEvent.Level.ToApprendaLogLevel());
        }
    }
}