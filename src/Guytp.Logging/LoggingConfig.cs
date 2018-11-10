using Guytp.Config;
using Newtonsoft.Json;

namespace Guytp.Logging
{
    /// <summary>
    /// This class provides the application logging configuration.
    /// </summary>
    public class LoggingConfig
    {
        #region Properties
        /// <summary>
        /// Gets the configuration for the whole application.
        /// </summary>
        public static LoggingConfig ApplicationInstance { get; }

        /// <summary>
        /// Gets or sets the console provider settings.
        /// </summary>
        [JsonProperty("console")]
        public LoggingConfigSection ConsoleSettings { get; set; }

        /// <summary>
        /// Gets or sets the event firing provider settings.
        /// </summary>
        [JsonProperty("eventFiring")]
        public LoggingConfigSection EventFiringSettings { get; set; }

        /// <summary>
        /// Gets or sets the file writing provider settings.
        /// </summary>
        [JsonProperty("fileWriting")]
        public FileWritingLoggingConfigSection FileWritingSettings { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Handle one time initialisation of this class.
        /// </summary>
        static LoggingConfig()
        {
            ApplicationInstance = AppConfig.ApplicationInstance.GetObject<LoggingConfig>("GuytpLogging");
            if (ApplicationInstance == null)
                ApplicationInstance = new LoggingConfig();
            if (ApplicationInstance.ConsoleSettings == null)
                ApplicationInstance.ConsoleSettings = new LoggingConfigSection();
            if (ApplicationInstance.EventFiringSettings == null)
                ApplicationInstance.EventFiringSettings = new LoggingConfigSection();
            if (ApplicationInstance.FileWritingSettings == null)
                ApplicationInstance.FileWritingSettings = new FileWritingLoggingConfigSection();
        }

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        private LoggingConfig()
        {
            ConsoleSettings = new LoggingConfigSection();
            EventFiringSettings = new LoggingConfigSection();
            FileWritingSettings = new FileWritingLoggingConfigSection();
        }
        #endregion
    }
}