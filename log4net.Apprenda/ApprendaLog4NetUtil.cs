namespace log4net.Apprenda
{
    using System.Collections.Generic;

    using global::Apprenda.Services.Logging;

    using log4net.Core;

    /// <summary>
    /// Provide conversion from the standard log4net logger levels to Apprenda Logging API LogLevels
    /// </summary>
    public static class ApprendaLog4NetUtil
    {
        /// <summary>
        /// The level map.
        /// </summary>
        private static readonly Dictionary<int, LogLevel> LevelMap = new Dictionary<int, LogLevel>()
        {
            { Level.Off.Value, LogLevel.Off },
            { Level.Fatal.Value, LogLevel.Fatal },
            { Level.Error.Value, LogLevel.Error },
            { Level.Warn.Value, LogLevel.Warn },
            { Level.Info.Value, LogLevel.Info },
            { Level.Debug.Value, LogLevel.Debug }
        };

        /// <summary>
        /// Return the appropriate Apprenda API LogLevel for a given logger Level
        /// </summary>
        /// <param name="level">
        /// The log4net Level.
        /// </param>
        /// <returns>
        /// The Apprenda <see cref="LogLevel"/>.
        /// </returns>
        public static LogLevel ToApprendaLogLevel(this Level level)
        {
            LogLevel result;
            if (!LevelMap.TryGetValue(level.Value, out result))
            {
                result = LogLevel.Debug; // if not a known log level, map to debug level.
            }

            return result;
        }
    }
}
