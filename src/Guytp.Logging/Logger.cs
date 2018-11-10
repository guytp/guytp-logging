using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Guytp.Logging
{
    /// <summary>
    /// This class is responsible for handling logging events within the system.
    /// </summary>
    /// <summary>
    /// This class provides logging capabilities for the application.
    /// </summary>
    public class Logger : IDisposable
    {
        #region Declarations
        /// <summary>
        /// Defines the application-wide logging instance.
        /// </summary>
        private static Logger _applicationInstance = null;

        /// <summary>
        /// Defines the lock object for thread access to the application-wide logging instance.
        /// </summary>
        private static readonly object ApplicationInstanceLocker = new object();

        /// <summary>
        /// Defines a list of all providers of logging capability.
        /// </summary>
        private readonly List<ILogProvider> _logProvidersInternal = new List<ILogProvider>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets a list of all providers of logging capability.
        /// </summary>
        public ILogProvider[] LogProviders
        {
            get
            {
                return _logProvidersInternal.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the application wide logging instance.  When setting the value any existing application wide instance will be disposed of.
        /// </summary>
        public static Logger ApplicationInstance
        {
            get
            {
                lock (ApplicationInstanceLocker)
                {
                    // Return default if available
                    if (_applicationInstance != null)
                        return _applicationInstance;

                    // No application instance has been defined so setup a default one now
                    Logger logger = new Logger();
                    LoggingConfigSection consoleConfiguration = LoggingConfig.ApplicationInstance.ConsoleSettings;
                    LoggingConfigSection eventFiringConfiguration = LoggingConfig.ApplicationInstance.EventFiringSettings;
                    FileWritingLoggingConfigSection fileWritingConfiguration = LoggingConfig.ApplicationInstance.FileWritingSettings;
                    if (consoleConfiguration != null && consoleConfiguration.LogLevels.Length > 0)
                        logger.AddLogProvider(new ConsoleLogProvider(consoleConfiguration.LogLevels));
                    if (eventFiringConfiguration != null && eventFiringConfiguration.LogLevels.Length > 0)
                        logger.AddLogProvider(new EventFiringLogProvider(eventFiringConfiguration.LogLevels));
                    if (fileWritingConfiguration != null && fileWritingConfiguration.LogLevels.Length > 0)
                    {
                        string logPath = !string.IsNullOrEmpty(fileWritingConfiguration.LogPath) ? fileWritingConfiguration.LogPath : Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                        if (!Directory.Exists(logPath))
                            try
                            {
                                Directory.CreateDirectory(logPath);
                            }
                            catch (Exception)
                            {
                                throw new Exception("Could not create log path '" + logPath + "'");
                            }
                        logger.AddLogProvider(new FileWritingLogProvider(fileWritingConfiguration.LogLevels, logPath, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + "-"));
                    }
                    _applicationInstance = logger;
                    return _applicationInstance;
                }
            }
            set
            {
                lock (ApplicationInstanceLocker)
                {
                    if (_applicationInstance != null)
                    {
                        _applicationInstance.Dispose();
                        _applicationInstance = null;
                    }
                    _applicationInstance = value;
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Fired when a log provider is added.
        /// </summary>
        public event EventHandler<LogProviderEventArgs> LogProviderAdded;

        /// <summary>
        /// Fired when a log provider is removed.
        /// </summary>
        public event EventHandler<LogProviderEventArgs> LogProviderRemoved;
        #endregion

        #region Logging
        /// <summary>
        /// Adds an error message to the log.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The optional exception to log.
        /// </param>
        /// <param name="memberName">
        /// The calling member name, obtained from CallerMemberName by default.
        /// </param>
        /// <param name="sourceFilePath">
        /// The calling source file, obtained from CallerFilePath by default.
        /// </param>
        /// <param name="sourceLineNumber">
        /// The calling line number, obtained from CallerLineNumber by default.
        /// </param>
        public void Error(string message, Exception exception = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Error, message, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Adds an informational message to the log.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The optional exception to log.
        /// </param>
        /// <param name="memberName">
        /// The calling member name, obtained from CallerMemberName by default.
        /// </param>
        /// <param name="sourceFilePath">
        /// The calling source file, obtained from CallerFilePath by default.
        /// </param>
        /// <param name="sourceLineNumber">
        /// The calling line number, obtained from CallerLineNumber by default.
        /// </param>
        public void Info(string message, Exception exception = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Info, message, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Adds a warning message to the log.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The optional exception to log.
        /// </param>
        /// <param name="memberName">
        /// The calling member name, obtained from CallerMemberName by default.
        /// </param>
        /// <param name="sourceFilePath">
        /// The calling source file, obtained from CallerFilePath by default.
        /// </param>
        /// <param name="sourceLineNumber">
        /// The calling line number, obtained from CallerLineNumber by default.
        /// </param>
        public void Warn(string message, Exception exception = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Warning, message, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Adds a debug message to the log.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The optional exception to log.
        /// </param>
        /// <param name="memberName">
        /// The calling member name, obtained from CallerMemberName by default.
        /// </param>
        /// <param name="sourceFilePath">
        /// The calling source file, obtained from CallerFilePath by default.
        /// </param>
        /// <param name="sourceLineNumber">
        /// The calling line number, obtained from CallerLineNumber by default.
        /// </param>
        public void Debug(string message, Exception exception = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Debug, message, exception, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Adds a trace message to the log.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        /// <param name="exception">
        /// The optional exception to log.
        /// </param>
        /// <param name="memberName">
        /// The calling member name, obtained from CallerMemberName by default.
        /// </param>
        /// <param name="sourceFilePath">
        /// The calling source file, obtained from CallerFilePath by default.
        /// </param>
        /// <param name="sourceLineNumber">
        /// The calling line number, obtained from CallerLineNumber by default.
        /// </param>
        public void Trace(string message, Exception exception = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Log(LogLevel.Trace, message, exception, memberName, sourceFilePath, sourceLineNumber);
        }
        #endregion

        #region Public Management
        /// <summary>
        /// Adds a log provider to those supported in the application.
        /// </summary>
        public void AddLogProvider(ILogProvider provider)
        {
            if (!_logProvidersInternal.Contains(provider))
                _logProvidersInternal.Add(provider);
            else
                return;
            LogProviderAdded?.Invoke(null, new LogProviderEventArgs(provider));
        }

        /// <summary>
        /// Removes a log provider from those supported in the application.
        /// </summary>
        public void RemoveLogProvider(ILogProvider provider)
        {
            if (_logProvidersInternal.Contains(provider))
                _logProvidersInternal.Remove(provider);
            else
                return;
            LogProviderRemoved?.Invoke(null, new LogProviderEventArgs(provider));
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Adds a message to the log.
        /// </summary>
        /// <param name="level">
        /// The log level.
        /// </param>
        /// <param name="message">
        /// The log message.
        /// </param>
        /// <param name="exception">
        /// The optional exception to log.
        /// </param>
        /// <param name="memberName">
        /// The calling member name.
        /// </param>
        /// <param name="sourceFilePath">
        /// The calling source file.
        /// </param>
        /// <param name="sourceLineNumber">
        /// The calling line number.
        /// </param>
        private void Log(LogLevel level, string message, Exception exception = null, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
        {
            LogEntry logEntry = new LogEntry(message, level, exception, memberName, sourceFilePath, sourceLineNumber, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name, DateTime.UtcNow);
            if (_logProvidersInternal != null)
                foreach (ILogProvider provider in _logProvidersInternal)
                    try
                    {
                        provider.AddLogEntry(logEntry);
                    }
                    catch
                    {
                        // Intentionally swallow
                    }
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Free up our resources.
        /// </summary>
        public void Dispose()
        {
            if (_logProvidersInternal != null)
                foreach (ILogProvider provider in _logProvidersInternal)
                {
                    IDisposable disposable = provider as IDisposable;
                    if (disposable == null)
                        continue;
                    disposable.Dispose();
                }
        }
        #endregion
    }
}