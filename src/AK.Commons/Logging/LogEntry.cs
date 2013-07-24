/*******************************************************************************************************************************
 * AK.Commons.Logging.LogEntry
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
    /// Contains information about a log entry. Constructed by the system
    /// implementation of IAppLogger using various inputs and consumed by
    /// the implementations of ILoggingProvider to actually log the thing.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class LogEntry
    {
        /// <summary>
        /// Application name (as passed in to AppEnvironment.Initialize).
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Instance of creation for this log entry.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Logging level.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// The method that asked for this entry to be logged (in
        /// Type.Method format).
        /// </summary>
        public string CallingMethod { get; set; }

        /// <summary>
        /// The actual message or description.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Returns a string that represents the current LogEntry instance.
        /// </summary>
        /// <returns>
        /// A string that represents the current LogEntry instance.
        /// </returns>
        public override string ToString()        
        {
            return string.Format("{0} {1} {2} {3} {4}", 
                this.TimeStamp, 
                this.ApplicationName, 
                this.LogLevel, 
                this.CallingMethod, 
                this.Message);
        }
    }
}