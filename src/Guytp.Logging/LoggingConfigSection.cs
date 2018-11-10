using Newtonsoft.Json;
using System.Collections.Generic;

namespace Guytp.Logging
{
    /// <summary>
    /// This class provides a basic configuration set for a generic log provider.
    /// </summary>
    public class LoggingConfigSection
    {
        #region Properties
        /// <summary>
        /// Gets the log levels this configuration element has enabled.
        /// </summary>
        [JsonIgnore]
        public LogLevel[] LogLevels
        {
            get
            {
                List<LogLevel> logLevels = new List<LogLevel>();
                if (Debug)
                    logLevels.Add(LogLevel.Debug);
                if (Info)
                    logLevels.Add(LogLevel.Info);
                if (Warning)
                    logLevels.Add(LogLevel.Warning);
                if (Error)
                    logLevels.Add(LogLevel.Error);
                if (Trace)
                    logLevels.Add(LogLevel.Trace);
                return logLevels.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets whether or not Trace log levels are enabled for this provider.
        /// </summary>
        [JsonProperty("trace")]
        public bool Trace { get; set; }

        /// <summary>
        /// Gets or sets whether or not Debug log levels are enabled for this provider.
        /// </summary>
        [JsonProperty("debug")]
        public bool Debug { get; set; }

        /// <summary>
        /// Gets or sets whether or not Info log levels are enabled for this provider.
        /// </summary>
        [JsonProperty("info")]
        public bool Info { get; set; }

        /// <summary>
        /// Gets or sets whether or not Warning log levels are enabled for this provider.
        /// </summary>
        [JsonProperty("warning")]
        public bool Warning { get; set; }

        /// <summary>
        /// Gets or sets whether or not Error log levels are enabled for this provider.
        /// </summary>
        [JsonProperty("error")]
        public bool Error { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        internal LoggingConfigSection()
        {
            Trace = false;
            Debug = false;
            Info = true;
            Warning = true;
            Error = true;
        }
        #endregion
    }
}