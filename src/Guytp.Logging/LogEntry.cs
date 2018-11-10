using System;
using System.IO;

namespace Guytp.Logging
{
    /// <summary>
    /// A LogEntry contains a single logging incident and all details related to it.
    /// </summary>
    public class LogEntry
    {
        #region Properties
        /// <summary>
        /// Gets the message for this logging statement.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the optional exception related to this logging statement.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the name of the calling member containing this logging statement.
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// Gets the path of the source file that this logging statement is contained within.
        /// </summary>
        public string SourceFilePath { get; }

        /// <summary>
        /// Gets the line of the source code file that this logging statement is contained within.
        /// </summary>
        public int SourceFileLineNumber { get; }

        /// <summary>
        /// Gets the level of loggign that this message represents.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// Gets the ID of the managed tread that this request came from.
        /// </summary>
        public int ThreadId { get; }
        
        /// <summary>
        /// Gets the name of the thread that this logging event came from.
        /// </summary>
        public string ThreadName { get; }

        /// <summary>
        /// Gets the log messages formatted as a detailed multi-line string.
        /// </summary>
        public string FormattedAsMultiline
        {
            get
            {
                string sourceFileNameWithoutExtension = string.Empty;
                if (!string.IsNullOrEmpty(SourceFilePath))
                    sourceFileNameWithoutExtension = Path.GetFileNameWithoutExtension(SourceFilePath);
                string messageWithLinesAndSpaces = "Message:     " + Message;
                messageWithLinesAndSpaces = messageWithLinesAndSpaces.Replace(Environment.NewLine, Environment.NewLine + "             ");
                string logMessage = $"Date:        {LogDate:yyyy-MM-dd HH:mm:ss}{Environment.NewLine}State:       {Level,-5}{Environment.NewLine}Location:    {sourceFileNameWithoutExtension}.{MemberName}:{SourceFileLineNumber}{Environment.NewLine}Thread:      {ThreadId} - {ThreadName}{Environment.NewLine}{messageWithLinesAndSpaces}";
                if (Exception != null)
                    logMessage += (Environment.NewLine + "Exception:   " + Exception).Replace(Environment.NewLine, Environment.NewLine + "             ");
                return logMessage;
            }
        }

        /// <summary>
        /// Gets the date/time in UTC of the logging event.
        /// </summary>
        public DateTime LogDate { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="message">
        /// The message for this logging statement.
        /// </param>
        /// <param name="level">
        /// The level of logging that this message represents.
        /// </param>
        /// <param name="exception">
        /// Tthe optional exception related to this logging statement.
        /// </param>
        /// <param name="memberName">
        /// The name of the calling member containing this logging statement.
        /// </param>
        /// <param name="sourceFilePath">
        /// The path of the source file that this logging statement is contained within.
        /// </param>
        /// <param name="sourceFileLineNumber">
        /// The line of the source code file that this logging statement is contained within.
        /// </param>
        /// <param name="threadId">
        /// The ID of the managed tread that this request came from.
        /// </param>
        /// <param name="threadName">
        /// The name of the thread that this logging event came from.
        /// </param>
        /// <param name="logDate">
        /// The date/time in UTC of the logging event.
        /// </param>
        public LogEntry(string message, LogLevel level, Exception exception, string memberName, string sourceFilePath, int sourceFileLineNumber, int threadId, string threadName, DateTime logDate)
        {
            Message = message;
            Level = level;
            Exception = exception;
            MemberName = memberName;
            SourceFilePath = sourceFilePath;
            SourceFileLineNumber = sourceFileLineNumber;
            ThreadId = threadId;
            ThreadName = threadName;
            LogDate = logDate;
        }
        #endregion
    }
}