using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Guytp.Logging
{
    /// <summary>
    /// The conosle log provider outputs log messages to the application console.
    /// </summary>
    public class ConsoleLogProvider : ILogProvider
    {
        #region Declarations
        /// <summary>
        /// Defines the levels of logging supported by this provider.
        /// </summary>
        private readonly LogLevel[] _supportedLogLevels;

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
        public ConsoleLogProvider(LogLevel[] supportedLogLevels)
        {
            if (supportedLogLevels == null || supportedLogLevels.Length == 0)
                throw new ArgumentNullException(nameof(supportedLogLevels), "No supported log levels");
            _supportedLogLevels = supportedLogLevels;
            _thread = new Thread(ThreadEntry)
            {
                IsBackground = true, // Worst case we don't want to keep a process up just due to logging
                Name = "Log Console Outputter"
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
            while (_isAlive)
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
                        Thread.Sleep(100);
                        continue;
                    }


                    foreach (LogEntry entry in entries)
                        Console.WriteLine(entry.FormattedAsMultiline);
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