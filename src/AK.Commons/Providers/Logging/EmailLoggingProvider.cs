/*******************************************************************************************************************************
 * AK.Commons.Providers.Logging.EmailLoggingProvider
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

using AK.Commons.Logging;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Mail;

#endregion

namespace AK.Commons.Providers.Logging
{
    /// <summary>
    /// Logging provider that sends out e-mails for logging.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [Export(typeof(ILoggingProvider))]
    public class EmailLoggingProvider : LoggingProviderBase
    {
        #region Constants

        private const string ConfigKeyFormatSmtpHost = "{0}.smtphost";
        private const string ConfigKeyFormatSmtpPort = "{0}.smtpport";
        private const string ConfigKeyFormatAuthenticate = "{0}.authenticate";
        private const string ConfigKeyFormatUserName = "{0}.username";
        private const string ConfigKeyFormatPassword = "{0}.password";
        private const string ConfigKeyFormatDomain = "{0}.domain";
        private const string ConfigKeyFormatTo = "{0}.to";
        private const string ConfigKeyFormatFrom = "{0}.from";
        private const string ConfigKeyFormatSubjectFormat = "{0}.subjectformat";
        private const string ConfigKeyFormatBodyFormat = "{0}.bodyformat";
        private const string ConfigKeyFormatIsBodyHtml = "{0}.isbodyhtml";
        private const int DefaultSmtpPort = 25;

        #endregion

        #region Properties (Private)

        private string ConfigKeySmtpHost { get { return string.Format(ConfigKeyFormatSmtpHost, this.ConfigKeyRoot); } }
        private string ConfigKeySmtpPort { get { return string.Format(ConfigKeyFormatSmtpPort, this.ConfigKeyRoot); } }
        private string ConfigKeyAuthenticate { get { return string.Format(ConfigKeyFormatAuthenticate, this.ConfigKeyRoot); } }
        private string ConfigKeyUserName { get { return string.Format(ConfigKeyFormatUserName, this.ConfigKeyRoot); } }
        private string ConfigKeyPassword { get { return string.Format(ConfigKeyFormatPassword, this.ConfigKeyRoot); } }
        private string ConfigKeyDomain { get { return string.Format(ConfigKeyFormatDomain, this.ConfigKeyRoot); } }
        private string ConfigKeyTo { get { return string.Format(ConfigKeyFormatTo, this.ConfigKeyRoot); } }
        private string ConfigKeyFrom { get { return string.Format(ConfigKeyFormatFrom, this.ConfigKeyRoot); } }
        private string ConfigKeySubjectFormat { get { return string.Format(ConfigKeyFormatSubjectFormat, this.ConfigKeyRoot); } }
        private string ConfigKeyBodyFormat { get { return string.Format(ConfigKeyFormatBodyFormat, this.ConfigKeyRoot); } }
        private string ConfigKeyIsBodyHtml { get { return string.Format(ConfigKeyFormatIsBodyHtml, this.ConfigKeyRoot); } }

        private string SmtpHost { get { return this.AppConfig.Get<string>(this.ConfigKeySmtpHost); } }
        private int SmtpPort { get { return this.AppConfig.Get(this.ConfigKeySmtpPort, DefaultSmtpPort); } }
        private bool Authenticate { get { return this.AppConfig.Get(this.ConfigKeyAuthenticate, false); } }
        private string UserName { get { return this.AppConfig.Get(this.ConfigKeyUserName, string.Empty); } }
        private string Password { get { return this.AppConfig.Get(this.ConfigKeyPassword, string.Empty); } }
        private string Domain { get { return this.AppConfig.Get(this.ConfigKeyDomain, string.Empty); } }
        private string To { get { return this.AppConfig.Get(this.ConfigKeyTo, string.Empty); } }
        private string From { get { return this.AppConfig.Get(this.ConfigKeyFrom, string.Empty); } }
        private string SubjectFormat { get { return this.AppConfig.Get(this.ConfigKeySubjectFormat, string.Empty); } }
        private string BodyFormat { get { return this.AppConfig.Get(this.ConfigKeyBodyFormat, string.Empty); } }
        private bool IsBodyHtml { get { return this.AppConfig.Get(this.ConfigKeyIsBodyHtml, false); } }

        #endregion

        #region Methods (LoggingProviderBase)

        protected override void LogEntry(LogEntry logEntry)
        {
            using (var client = new SmtpClient(this.SmtpHost, this.SmtpPort))
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(this.From);
                message.To.Add(this.To);
                message.Subject = logEntry.ToFormattedString(this.SubjectFormat);
                message.Body = logEntry.ToFormattedString(this.BodyFormat);
                message.IsBodyHtml = this.IsBodyHtml;

                if (this.Authenticate)
                {
                    client.EnableSsl = true;
                    client.Credentials = string.IsNullOrWhiteSpace(this.Domain)
                                             ? new NetworkCredential(this.UserName, this.Password)
                                             : new NetworkCredential(this.UserName, this.Password, this.Domain);
                }

                client.Send(message);
            }
        }

        #endregion
    }
}