namespace Guytp.Logging
{
    /// <summary>
    /// Defines the various evels that log messages can be entered at.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Indicates a log representing debug information.
        /// </summary>
        Debug,

        /// <summary>
        /// Indicates a log representing general information.
        /// </summary>
        Info,

        /// <summary>
        /// Indicates a log representing a warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Indicates a log representing an error.
        /// </summary>
        Error,

        /// <summary>
        /// Indicates a log representing trace information.
        /// </summary>
        Trace
    }
}