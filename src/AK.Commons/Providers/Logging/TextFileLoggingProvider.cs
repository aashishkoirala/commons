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
using System.IO;
using System.Reflection;
using System.Web;
using AK.Commons.Configuration;
using AK.Commons.Logging;

#endregion

namespace AK.Commons.Providers.Logging
{
    /// <summary>
    /// Logging provider that uses text files for logging.
    /// </summary>
    /// <author>Aashish Koirala</author>
    /// <todo>
    ///  1) Support rotation of log files.
    ///  2) Support date-stamp in file names.
    /// </todo>
    [Export(typeof (ILoggingProvider))]
    public class TextFileLoggingProvider : ILoggingProvider
    {
        #region Constants/Fields

        private const string ConfigKeyEnabled = "ak.commons.providers.logging.textfileloggingprovider.enabled";
        private const string ConfigKeyMessageFormat =
            "ak.commons.providers.logging.textfileloggingprovider.messageformat";
        private const string ConfigKeyFilePath = "ak.commons.providers.logging.textfileloggingprovider.filepath";
        private const string ConfigKeyFileNameFormat = 
            "ak.commons.providers.logging.textfileloggingprovider.filenameformat";
        private const string DefaultFileNameFormat = "ApplicationLog.log";

        private string fileName;
        private readonly HttpContext httpContext;

        [Import] private Lazy<IAppConfig> appConfig;

        #endregion

        #region Constructor

        [ImportingConstructor]
        public TextFileLoggingProvider()
        {
            this.httpContext = HttpContext.Current;
        }

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

        private string FilePath
        {
            get
            {
                var filePath = this.AppConfig.Get(ConfigKeyFilePath, string.Empty);
                if (string.IsNullOrWhiteSpace(filePath))
                    filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                ManageWebRelativePath(this.httpContext, ref filePath);
                ManageRelativePath(ref filePath);

                return filePath;
            }
        }

        private string FileNameFormat
        {
            get { return this.AppConfig.Get(ConfigKeyFileNameFormat, DefaultFileNameFormat); }
        }

        private string FileName
        {
            get
            {
                if (this.fileName == null)
                {
                    var fileNameFormat = this.FileNameFormat;
                    Path.GetInvalidFileNameChars().ForEach(x => fileNameFormat = fileNameFormat.Replace(x, '_'));
                    this.fileName = Path.Combine(this.FilePath, this.FileNameFormat);
                    this.fileName = Path.GetFullPath(this.fileName);
                    Path.GetInvalidPathChars().ForEach(x => this.fileName = this.fileName.Replace(x, '_'));
                }
                return this.fileName;
            }
        }

        #endregion

        #region Methods (ILoggingProvider)

        public void Log(LogEntry logEntry)
        {            
            if (!this.Enabled) return;

            var message = string.IsNullOrWhiteSpace(this.MessageFormat) ? 
                logEntry.ToString() : logEntry.ToFormattedString(this.MessageFormat);
            message += Environment.NewLine;

            File.AppendAllText(this.FileName, message);
        }

        #endregion

        #region Methods (Private)

        private static void ManageWebRelativePath(HttpContext context, ref string path)
        {
            if (!path.StartsWith("~/")) return;
            if (context == null) return;

            path = context.Server.MapPath(path);
        }

        private static void ManageRelativePath(ref string path)
        {
            if (Path.IsPathRooted(path)) return;

            var entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var entryAssemblyDirectory = (Path.GetDirectoryName(entryAssembly.Location) ?? string.Empty).ToLower();
            path = Path.GetFullPath(Path.Combine(entryAssemblyDirectory, path));
        }

        #endregion
    }
}