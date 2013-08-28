/*******************************************************************************************************************************
 * AK.Commons.Providers.Logging.ConsoleLoggingProvider
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
using System.ComponentModel.Composition;
using AK.Commons.Logging;

#endregion

namespace AK.Commons.Providers.Logging
{
    /// <summary>
    /// Logging provider that uses the Console for logging.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (ILoggingProvider))]
    public class ConsoleLoggingProvider : LoggingProviderBase
    {
        #region Constants/Fields

        private const string ConfigKeyFormatMessageFormat = "{0}.messageformat";

        #endregion

        #region Properties (Private)

        private string ConfigKeyMessageFormat
        {
            get { return string.Format(ConfigKeyFormatMessageFormat, this.ConfigKeyRoot); }
        }

        private string MessageFormat
        { 
            get { return this.AppConfig.Get(this.ConfigKeyMessageFormat, string.Empty); }
        }

        #endregion

        #region Methods (LoggingProviderBase)

        protected override void LogEntry(LogEntry logEntry)
        {
            var message = string.IsNullOrWhiteSpace(this.MessageFormat) ? 
                logEntry.ToString() : logEntry.ToFormattedString(this.MessageFormat);

            Console.WriteLine(message);
        }

        #endregion
    }
}