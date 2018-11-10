using System;
using System.Linq;

namespace Guytp.Logging
{
    /// <summary>
    /// The event firing log provider fires events within the application whenever a logging statement is made.
    /// </summary>
    public class EventFiringLogProvider : ILogProvider
    {
        #region Declarations
        /// <summary>
        /// Defines the levels of logging supported by this provider.
        /// </summary>
        private readonly LogLevel[] _supportedLogLevels;
        #endregion

        #region Events
        /// <summary>
        /// Fired when a log entry has been received.
        /// </summary>
        public event EventHandler<LogEventArgs> LogReceived;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="supportedLogLevels">
        /// The levels of logging supported by this provider.
        /// </param>
        public EventFiringLogProvider(LogLevel[] supportedLogLevels)
        {
            if (supportedLogLevels == null || supportedLogLevels.Length == 0)
                throw new ArgumentNullException(nameof(supportedLogLevels), "No supported log levels");
            _supportedLogLevels = supportedLogLevels;
        }
        #endregion

        /// <summary>
        /// Indicates a new log entry has been received and that the log provider should handle it.
        /// </summary>
        /// <param name="logEntry">
        /// The log entry to handle.
        /// </param>
        public void AddLogEntry(LogEntry logEntry)
        {
            if (!_supportedLogLevels.Contains(logEntry.Level))
                return;
            try
            {
                LogReceived?.Invoke(null, new LogEventArgs(logEntry));
            }
            catch
            {
                // Don't let logging bring us down so swallow the error
            }
        }
    }
}