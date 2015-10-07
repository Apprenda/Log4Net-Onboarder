// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApprendaLog4NetUtil.cs" company="Apprenda, Inc.">
//   The MIT License (MIT)
//   
//   Copyright (c) 2015 Apprenda Inc.
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//   SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
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
            {
                Level.Off
                    .Value,
                LogLevel
                    .Off
            },
            {
                Level.Fatal
                    .Value,
                LogLevel
                    .Fatal
            },
            {
                Level.Error
                    .Value,
                LogLevel
                    .Error
            },
            {
                Level.Warn
                    .Value,
                LogLevel
                    .Warn
            },
            {
                Level.Info
                    .Value,
                LogLevel
                    .Info
            },
            {
                Level.Debug
                    .Value,
                LogLevel
                    .Debug
            }
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