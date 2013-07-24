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
using AK.Commons.Configuration;
using AK.Commons.Logging;

#endregion

namespace AK.Commons.Providers.Logging
{
    /// <summary>
    /// Logging provider that uses the Console for logging.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof (ILoggingProvider))]
    public class ConsoleLoggingProvider : ILoggingProvider
    {
        #region Constants/Fields

        private const string ConfigKeyEnabled = "ak.commons.providers.logging.consoleloggingprovider.enabled";
        private const string ConfigKeyMessageFormat =
            "ak.commons.providers.logging.consoleloggingprovider.messageformat";

        [Import] private Lazy<IAppConfig> appConfig;

        #endregion

        #region Properties (Private)

        private IAppConfig AppConfig {get { return this.appConfig.Value; }}

        private bool Enabled
        {
            get { return this.AppConfig.Get(ConfigKeyEnabled, false); }
        }

        private string MessageFormat
        { 
            get { return this.AppConfig.Get(ConfigKeyMessageFormat, string.Empty); }
        }

        #endregion

        #region Methods (ILoggingProvider)

        public void Log(LogEntry logEntry)
        {
            if (!this.Enabled) return;

            var message = string.IsNullOrWhiteSpace(this.MessageFormat) ? 
                logEntry.ToString() : logEntry.ToFormattedString(this.MessageFormat);

            Console.WriteLine(message);
        }

        #endregion
    }
}