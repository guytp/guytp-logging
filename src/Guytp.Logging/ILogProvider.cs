namespace Guytp.Logging
{
    /// <summary>
    /// A log provider takes logging information and then outputs it to an appropriate source.
    /// </summary>
    public interface ILogProvider
    {
        /// <summary>
        /// Indicates a new log entry has been received and that the log provider should handle it.
        /// </summary>
        /// <param name="logEntry">
        /// The log entry to handle.
        /// </param>
        void AddLogEntry(LogEntry logEntry);
    }
}