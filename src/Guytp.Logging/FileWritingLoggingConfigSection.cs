using Newtonsoft.Json;

namespace Guytp.Logging
{
    /// <summary>
    /// This class provides a basic configuration set for a generic log provider.
    /// </summary>
    public class FileWritingLoggingConfigSection : LoggingConfigSection
    {
        #region Properties
        /// <summary>
        /// Gets or sets the path to write log files to.
        /// </summary>
        [JsonProperty("log_path")]
        public string LogPath { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        internal FileWritingLoggingConfigSection()
        {
            LogPath = string.Empty;
        }
        #endregion
    }
}