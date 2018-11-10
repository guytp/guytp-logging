using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Guytp.Logging
{
    /// <summary>
    /// The file writing log provider writes to disk whenever a log statement is made.
    /// </summary>
    public class FileWritingLogProvider : ILogProvider, IDisposable
    {
        #region Declarations
        /// <summary>
        /// Defines the levels of logging supported by this provider.
        /// </summary>
        private readonly LogLevel[] _supportedLogLevels;

        /// <summary>
        /// Defines the path to write log file.
        /// </summary>
        private string _logPath;

        /// <summary>
        /// Defines the file prefix for log files which will be followed with "yyyy-MM-dd.log"
        /// </summary>
        private string _logFilePrefix;

        /// <summary>
        /// Defines the list of log entries we have not yet output.
        /// </summary>
        private List<LogEntry> _logEntries = new List<LogEntry>();

        /// <summary>
        /// Defines the locking object we use for thread saftey.
        /// </summary>
        private readonly object _locker = new object();

        /// <summary>
        /// Defines the thread we use for writing log files.
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// Defines whether or not the log thread is alive.
        /// </summary>
        private bool _isAlive;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="supportedLogLevels">
        /// The levels of logging supported by this provider.
        /// </param>
        /// <param name="logPath">
        /// The path to write log file.
        /// </param>
        /// <param name="logFilePrefix">
        /// The file prefix for log files which will be followed with "yyyy-MM-dd.log"
        /// </param>
        public FileWritingLogProvider(LogLevel[] supportedLogLevels, string logPath, string logFilePrefix)
        {
            if (supportedLogLevels == null || supportedLogLevels.Length == 0)
                throw new ArgumentNullException(nameof(supportedLogLevels), "No supported log levels");
            _supportedLogLevels = supportedLogLevels;
            _logFilePrefix = logFilePrefix;
            _logPath = logPath;
            _thread = new Thread(ThreadEntry)
            {
                IsBackground = true, // Worst case we don't want to keep a process up just due to logging
                Name = "Log File Writer"
            };
            _isAlive = true;
            _thread.Start();
        }
        #endregion

        /// <summary>
        /// This method contains the main entry point for logging.
        /// </summary>
        private void ThreadEntry()
        {
            DateTime openLogDate = new DateTime(0);
            FileStream fileStream = null;

            while (true)
            {
                LogEntry[] entries = null;
                try
                {
                    // Get handle to log entries
                    lock (_locker)
                    {
                        entries = _logEntries.ToArray();
                        _logEntries.Clear();
                    }
                    if (entries.Length < 1)
                    {
                        if (!_isAlive)
                            break;
                        Thread.Sleep(100);
                        continue;
                    }


                    StringBuilder sb = new StringBuilder();
                    foreach (LogEntry entry in entries)
                    {
                        // Open a log file for this entry if it isn't already open
                        if (fileStream == null || entry.LogDate.Year != openLogDate.Year || entry.LogDate.Month != openLogDate.Month || entry.LogDate.Day != openLogDate.Day)
                        {
                            if (fileStream != null)
                            {
                                fileStream.Flush();
                                fileStream.Close();
                                fileStream.Dispose();
                            }
                            fileStream = File.Open(Path.Combine(_logPath, _logFilePrefix + entry.LogDate.ToString("yyyy-MM-dd") + ".log"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                            fileStream.Seek(0, SeekOrigin.End);
                            openLogDate = entry.LogDate;
                        }

                        // Append the log entry
                        sb.Length = 0;
                        string exception = entry.Exception?.ToString()?.Replace("\r", "\\r").Replace("\n", "\\n").Replace("|", string.Empty);
                        sb.AppendFormat("{0}|{1}|{2}|{3}|{4}|{6}|{5}|{7}|{8}\r\n", entry.LogDate.ToString("yyyy-MM-dd HH:mm:ss"), entry.Level, entry.ThreadId, entry.ThreadName, entry.SourceFilePath, entry.SourceFileLineNumber, entry.MemberName, entry.Message.Replace("\r", "\\r").Replace("\n", "\\n").Replace("|", string.Empty), exception);
                        byte[] logBytes = Encoding.UTF8.GetBytes(sb.ToString());
                        fileStream.Write(logBytes, 0, logBytes.Length);
                    }
                    fileStream?.Flush();
                }
                catch
                {
                    // We don't want to bring down logger, so just add this back to our list unless it's beyond a maximum size
                    if (entries != null)
                        try
                        {
                            lock (_locker)
                                _logEntries.InsertRange(0, entries);
                        }
                        catch
                        {
                            // Definitely swallow this time
                        }
                }

                // Wait to retry
                Thread.Sleep(50);
            }

            try
            {
                fileStream?.Dispose();
            }
            catch
            {
                // Swallow silently
            }
        }

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
                lock (_locker)
                    _logEntries.Add(logEntry);
            }
            catch
            {
                // Don't let logging bring us down so swallow the error
            }
        }

        #region IDisposable Implementation
        /// <summary>
        /// Free up our resources.
        /// </summary>
        public void Dispose()
        {
            _isAlive = false;
            _thread?.Join();
            _thread = null;
        }
        #endregion
    }
}