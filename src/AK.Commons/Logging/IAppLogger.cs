/*******************************************************************************************************************************
 * AK.Commons.Logging.IAppLogger
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

using System;

namespace AK.Commons.Logging
{
    /// <summary>
    /// Interface that provides logging operations to the application. You should not directly implement
    /// this interface or register another MEF export for this. The application environment expects only
    /// one implementation for this which it provides. To implement your own logging, you would create
    /// a new logging provider by implementing <see cref="ILoggingProvider"/> and register it using
    /// configuration.
    /// 
    /// In order to use the system implementation of this, you need to:
    /// 1) Make sure you've called AppEnvironment.Initialize().
    /// 2) Either:
    ///    a) Create a MEF import against IAppLogger (preferably), or:
    ///    b) Use AppEnvironment.Logger.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IAppLogger
    {
        /// <summary>
        /// Logs the given message with the given level.
        /// </summary>
        /// <param name="logLevel">Logging level.</param>
        /// <param name="message">Message to log.</param>
        void Log(LogLevel logLevel, string message);

        /// <summary>
        /// Logs the given message with Diagnostic level.
        /// </summary>
        /// <param name="message">Message to log.</param>
        void Diagnostic(string message);

        /// <summary>
        /// Logs the given message with Verbose level.
        /// </summary>
        /// <param name="message">Message to log.</param>
        void Verbose(string message);

        /// <summary>
        /// Logs the given message with Information level.
        /// </summary>
        /// <param name="message">Message to log.</param>
        void Information(string message);

        /// <summary>
        /// Logs the given message with Warning level.
        /// </summary>
        /// <param name="message">Message to log.</param>
        void Warning(string message);

        /// <summary>
        /// Logs the given message with Error level.
        /// </summary>
        /// <param name="message">Message to log.</param>
        void Error(string message);

        /// <summary>
        /// Logs the given exception with Warning level.
        /// </summary>
        /// <param name="exception">Exception to log.</param>
        void Warning(Exception exception);

        /// <summary>
        /// Logs the given exception with Error level.
        /// </summary>
        /// <param name="exception">Exception to log.</param>
        void Error(Exception exception);
    }
}