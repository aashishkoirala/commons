/*******************************************************************************************************************************
 * AK.Commons.Logging.ILoggingProvider
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

namespace AK.Commons.Logging
{
    /// <summary>
    /// Interface to be implemented by logging providers. Creating a logging provider involves implementing this interface,
    /// making it a MEF export for this interface; and then registering it using configuration so that IAppLogger then
    /// picks it up while logging.
    /// </summary>
    public interface ILoggingProvider
    {
        /// <summary>
        /// Logs the given entry.
        /// </summary>
        /// <param name="logEntry">LogEntry object with information about the event to log.</param>
        void Log(LogEntry logEntry);
    }
}