/*******************************************************************************************************************************
 * AK.Commons.Providers.Logging.AppLogger
 * Copyright © 2013 Aashish Koirala <http://aashishkoirala.github.io>
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using AK.Commons.Composition;
using AK.Commons.Exceptions;
using AK.Commons.Logging;

#endregion

namespace AK.Commons.Providers.Logging
{
    /// <summary>
    /// The one and only internal implementation of IAppLogger.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class AppLogger : IAppLogger
    {
        #region Constants/Fields

        private const string LogLevelConfigKey = "ak.commons.logging.loglevel";

        private readonly static BlockingCollection<LogEntry> LogEntries = new BlockingCollection<LogEntry>();

        private readonly IList<ILoggingProvider> loggingProviders;
        private readonly ManualResetEvent logProcessingStopEvent = new ManualResetEvent(false);
        private readonly string applicationName;

        #endregion

        #region Constructor

        public AppLogger(string applicationName, IComposer composer, IAppLogger logger)
        {
            this.applicationName = applicationName;
            this.loggingProviders = composer.ResolveMany<ILoggingProvider>().ToList();

            var loggingProviderLogBuilder = new StringBuilder(
                string.Format("Initializing logging for application {0}{1}Providers:{1}", 
                this.applicationName, Environment.NewLine));

            this.loggingProviders.ForEach(x => 
                loggingProviderLogBuilder.AppendFormat("{0}{1}", x.GetType().FullName, Environment.NewLine));

            logger.Information(loggingProviderLogBuilder.ToString());

            // TODO: Consider using TPL.
            //
            ThreadPool.QueueUserWorkItem(state =>
            {
                logger.Verbose("Log queue processing thread started.");

                while (true)
                {
                    if (!ProcessLogQueue(state as LoggerState))
                        break;
                }
            }, new LoggerState
            {
                LoggingProviders = this.loggingProviders, 
                LogProcessingStopEvent = this.logProcessingStopEvent
            });
        }

        #endregion

        #region Methods (ShutDown - used only by AppEnvironment)

        public void ShutDown()
        {
            this.Verbose("Shutting down logging.");
            this.logProcessingStopEvent.Set();
            this.Log(string.Empty, LogLevel.Information, string.Empty);
        }

        #endregion

        #region Methods (IAppLogger)

        public void Log(LogLevel logLevel, string message)
        {
            this.Log(GetCallingMethod(), logLevel, message);
        }

        public void Diagnostic(string message)
        {
            this.Log(GetCallingMethod(), LogLevel.Diagnostic, message);
        }

        public void Verbose(string message)
        {
            this.Log(GetCallingMethod(), LogLevel.Verbose, message);
        }

        public void Information(string message)
        {
            this.Log(GetCallingMethod(), LogLevel.Information, message);
        }

        public void Warning(string message)
        {
            this.Log(GetCallingMethod(), LogLevel.Warning, message);
        }

        public void Error(string message)
        {
            this.Log(GetCallingMethod(), LogLevel.Error, message);
        }

        public void Warning(Exception exception)
        {
            this.Log(GetCallingMethod(), LogLevel.Warning, GetMessageForException(exception));
        }

        public void Error(Exception exception)
        {
            this.Log(GetCallingMethod(), LogLevel.Error, GetMessageForException(exception));
        }

        #endregion

        #region Methods (Private)

        private void Log(string callingMethod, LogLevel logLevel, string message)
        {
            var configuredLogLevel = AppEnvironment.Config.Get(LogLevelConfigKey, LogLevel.Verbose);

            if (!GetMatchingLogLevels(configuredLogLevel).Contains(logLevel)) return;

            var logEntry = new LogEntry
            {
                TimeStamp = DateTime.Now, 
                ApplicationName = this.applicationName,
                Message = message, 
                LogLevel = logLevel, 
                CallingMethod = callingMethod
            };

            LogEntries.Add(logEntry);
        }
        
        private static IEnumerable<LogLevel> GetMatchingLogLevels(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Diagnostic:
                    return new[] {LogLevel.Diagnostic, LogLevel.Verbose, LogLevel.Information, LogLevel.Warning, LogLevel.Error };

                case LogLevel.Verbose:
                    return new[] {LogLevel.Verbose, LogLevel.Information, LogLevel.Warning, LogLevel.Error};

                case LogLevel.Information:
                    return new[] {LogLevel.Information, LogLevel.Warning, LogLevel.Error};

                case LogLevel.Warning:
                    return new[] {LogLevel.Warning, LogLevel.Error};

                case LogLevel.Error:
                    return new[] {LogLevel.Error};
            }

            return Enumerable.Empty<LogLevel>();
        }

        private static string GetCallingMethod()
        {
            var stackTrace = new StackTrace();
            if (stackTrace.FrameCount < 3) return string.Empty;

            var methodInfo = stackTrace.GetFrame(2).GetMethod();
            var type = methodInfo.DeclaringType == null ? 
                string.Empty : methodInfo.DeclaringType.FullName;

            return string.Format("{0}.{1}", type, methodInfo.Name);
        }

        private static string GetMessageForException(Exception ex)
        {
            if (ex is ISerializableException)
                return (ex as ISerializableException).SerializedData;

            return ex.ToString();
        }

        private static bool ProcessLogQueue(LoggerState state)
        {
            var loggingProviders = state.LoggingProviders;
            var logProcessingStopEvent = state.LogProcessingStopEvent;

            var logEntry = LogEntries.Take();

            var signaled = logProcessingStopEvent.WaitOne(100);
            if (signaled) return false;

            // ReSharper disable EmptyGeneralCatchClause
            //
            try
            {
                loggingProviders.ForEach(x => x.Log(logEntry));
            }
            catch {}
            //
            // ReSharper restore EmptyGeneralCatchClause

            return true;
        }

        #endregion

        #region Internal Type (LoggerState)

        private class LoggerState
        {
            public IList<ILoggingProvider> LoggingProviders { get; set; }
            public ManualResetEvent LogProcessingStopEvent { get; set; }
        }

        #endregion
    }
}