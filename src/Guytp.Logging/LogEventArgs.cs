using System;

namespace Guytp.Logging
{
    /// <summary>
    /// This class encapsulates a LogEntry for use in an event.
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        #region Properties
        /// <summary>
        /// Gets the log entry attached to this event.
        /// </summary>
        public LogEntry LogEntry { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="logEntry">
        /// The log entry attached to this event.
        /// </param>
        public LogEventArgs(LogEntry logEntry)
        {
            LogEntry = logEntry;
        }
        #endregion
    }
}