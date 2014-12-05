/*******************************************************************************************************************************
 * AK.Commons.Logging.LoggingProvider
 * Copyright © 2013-2014 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Library (AKCL).
 *  
 * AKCL is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCL is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCL.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using AK.Commons.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

#endregion

namespace AK.Commons.Logging
{
    #region ILoggingProvider

    /// <summary>
    /// Interface to be implemented by logging providers. Creating a logging provider involves implementing this interface,
    /// making it a MEF export for this interface; and then registering it using configuration so that IAppLogger then
    /// picks it up while logging.
    /// </summary>
    public interface ILoggingProvider
    {
        /// <summary>
        /// Gets whether this provider is enabled.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Logs the given entry.
        /// </summary>
        /// <param name="logEntry">LogEntry object with information about the event to log.</param>
        void Log(LogEntry logEntry);

        /// <summary>
        /// Gets whether this provider is enabled for the given logging level.
        /// </summary>
        /// <param name="logLevel">Logging level.</param>
        /// <returns>Enabled?</returns>
        bool IsEnabledForLevel(LogLevel logLevel);
    }

    #endregion

    #region LoggingProviderBase

    /// <summary>
    /// Base class for logging providers: provides common functionality.
    /// Consider inheriting this and implementing LogEntry rather than
    /// directly implementing ILoggingProvider.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public abstract class LoggingProviderBase : ILoggingProvider
    {
        #region Constants

        private const string ConfigKeyEnabledFormat = "{0}.enabled";
        private const string ConfigKeyLogLevelFilterFormat = "{0}.loglevelfilter";

        #endregion

        #region Fields

        [Import] protected Lazy<IAppConfig> appConfigLazy;

        private bool? enabled;
        private IDictionary<LogLevel, bool> enabledByLogLevelHash;

        #endregion

        #region Properties/Methods (ILoggingProvider)

        public virtual bool Enabled
        {
            get
            {
                if (this.enabled.HasValue) return this.enabled.Value;

                var enabledConfigKey = string.Format(ConfigKeyEnabledFormat, this.ConfigKeyRoot);
                this.enabled = this.AppConfig.Get(enabledConfigKey, false);

                return this.enabled.Value;
            }
        }

        public void Log(LogEntry logEntry)
        {
            if (!this.Enabled) return;
            if (!this.IsEnabledForLevel(logEntry.LogLevel)) return;

            this.LogEntry(logEntry);
        }

        public virtual bool IsEnabledForLevel(LogLevel logLevel)
        {
            if (this.enabledByLogLevelHash == null)
            {
                this.enabledByLogLevelHash = Enum.GetValues(typeof (LogLevel))
                                                 .Cast<LogLevel>()
                                                 .ToDictionary(x => x, x => false);

                var logLevelFilterConfigKey = string.Format(ConfigKeyLogLevelFilterFormat, this.ConfigKeyRoot);
                var logLevelFilter = this.AppConfig.Get(logLevelFilterConfigKey, string.Empty);

                if (string.IsNullOrWhiteSpace(logLevelFilter))
                {
                    var logLevels = this.enabledByLogLevelHash.Keys.ToList();

                    foreach (var level in logLevels)
                        this.enabledByLogLevelHash[level] = true;
                }
                else
                {
                    var logLevels = logLevelFilter
                        .Split(new[] {','})
                        .Select(x => (LogLevel) Enum.Parse(typeof (LogLevel), x))
                        .ToList();

                    foreach (var level in logLevels)
                        this.enabledByLogLevelHash[level] = true;
                }
            }

            bool returnValue;

            return this.enabledByLogLevelHash.TryGetValue(logLevel, out returnValue) && returnValue;
        }

        #endregion

        #region Properties/Methods (Protected)

        protected IAppConfig AppConfig
        {
            get { return this.appConfigLazy.Value; }
        }

        /// <summary>
        /// Implement actual logging routine in this method.
        /// </summary>
        /// <param name="logEntry">LogEntry object.</param>
        protected abstract void LogEntry(LogEntry logEntry);

        protected virtual string ConfigKeyRoot
        {
            get
            {
                var typeName = this.GetType().FullName;
                Debug.Assert(typeName != null);

                return typeName.ToLower();
            }
        }

        #endregion
    }

    #endregion
}