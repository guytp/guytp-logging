using System;

namespace Guytp.Logging
{
    /// <summary>
    /// These event arguments are used to pass around an instance of a LogProvider.
    /// </summary>
    public class LogProviderEventArgs : EventArgs
    {
        #region Properties
        /// <summary>
        /// Gets the log provider this event relates to.
        /// </summary>
        public ILogProvider LogProvider { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        /// <param name="logProvider">
        /// The log provider this event relates to.
        /// </param>
        public LogProviderEventArgs(ILogProvider logProvider)
            : base()
        {
            LogProvider = logProvider;
        }
        #endregion
    }
}