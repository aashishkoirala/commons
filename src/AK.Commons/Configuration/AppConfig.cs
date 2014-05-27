/*******************************************************************************************************************************
 * AK.Commons.Configuration.AppConfig
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

using AK.Commons.Configuration.Sections;
using AK.Commons.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

#endregion

namespace AK.Commons.Configuration
{
    #region IAppConfig

    /// <summary>
    /// Interface that provides configuration services to the application. You should not directly implement
    /// this interface or register another MEF export for this. The application environment expects only
    /// one implementation for this which it provides. You can implement your own configuration store
    /// by implementing <see cref="IConfigStore"/> and use that though.
    /// 
    /// In order to use the system implementation of this, you need to:
    /// 1) Make sure you've called AppEnvironment.Initialize().
    /// 2) Either:
    ///    a) Create a MEF import against IAppConfig (preferably), or:
    ///    b) Use AppEnvironment.Config.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IAppConfig
    {
        /// <summary>
        /// Gets the value of the given configuration setting.
        /// </summary>
        /// <typeparam name="TItem">Setting type.</typeparam>
        /// <param name="key">Setting name or key.</param>
        /// <returns>Setting value.</returns>
        /// <exception cref="AppConfigException">If key is not found or is of the wrong type.</exception>
        TItem Get<TItem>(string key);

        /// <summary>
        /// Gets the value of the given configuration setting, or a default value if not found.
        /// </summary>
        /// <typeparam name="TItem">Setting type.</typeparam>
        /// <param name="key">Setting name or key.</param>
        /// <param name="defaultValue">The default value to use if the setting is not found.</param>
        /// <returns>Setting value.</returns>
        /// <exception cref="AppConfigException">If key is of the wrong type.</exception>
        TItem Get<TItem>(string key, TItem defaultValue);

        /// <summary>
        /// Tries to get the value of the given configuration setting.
        /// </summary>
        /// <typeparam name="TItem">Setting type.</typeparam>
        /// <param name="key">Setting name or key.</param>
        /// <param name="value">Out parameter to receive the value in.</param>
        /// <returns>Whether the value was found and retrieved.</returns>
        bool TryGet<TItem>(string key, out TItem value);
    }

    #endregion

    #region AppConfig

    /// <summary>
    /// The one and only internal implementation of IAppConfig.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class AppConfig : IAppConfig
    {
        #region Constants/Fields

        private const string ConfigTempFileNameFormat = "AKConfig_{0}.xml";
        private const string TempFolderEnvironmentVariable = "TEMP";
        private const string ConfigSectionName = "ak.commons.configuration";

        private readonly IDictionary<string, object> settings;

        #endregion

        #region Constructor/Properties

        public AppConfig(string applicationName, IConfigStore configStore, IAppLogger logger, string tempLocation)
        {
            var untokenifiedXml = configStore.GetConfigurationXml();
            var untokenifiedConfiguration = GetConfiguration(untokenifiedXml, tempLocation);

            var section = untokenifiedConfiguration.GetSection(ConfigSectionName)
                as ApplicationSettingsConfigurationSection;

            Debug.Assert(section != null);

            var globalSettings = section.ApplicationSettings.Applications[string.Empty];
            var applicationSettings = section.ApplicationSettings.Applications[applicationName];

            var tokens = GetCombinedTokens(globalSettings.Tokens.Tokens, applicationSettings.Tokens.Tokens);
            var tokenifiedXml = GetTokenifiedXml(untokenifiedXml, tokens);
            var tokenifiedConfiguration = GetConfiguration(tokenifiedXml, tempLocation);
            var processedConfiguration = ProcessConfiguration(tokenifiedConfiguration, applicationName);

            this.settings = processedConfiguration;
            this.Configuration = tokenifiedConfiguration;
        }

        public System.Configuration.Configuration Configuration { get; private set; }

        #endregion

        #region Methods (IAppConfig)

        public TItem Get<TItem>(string key)
        {
            object item;
            var exists = this.settings.TryGetValue(key, out item);

            Debug.Assert(exists);
            Debug.Assert(item != null);
            Debug.Assert(item is TItem);

            return (TItem)item;
        }

        public TItem Get<TItem>(string key, TItem defaultValue)
        {
            TItem value;
            return this.TryGet(key, out value) ? value : defaultValue;
        }

        public bool TryGet<TItem>(string key, out TItem value)
        {
            value = default(TItem);

            object item;
            var exists = this.settings.TryGetValue(key, out item);

            if (!exists) return false;

            Debug.Assert(item != null);
            Debug.Assert(item is TItem);

            value = (TItem)item;
            return true;
        }

        #endregion

        #region Methods (Private Helper)

        private static System.Configuration.Configuration GetConfiguration(string xml, string tempLocation)
        {
            var fileName = string.Format(ConfigTempFileNameFormat, Guid.NewGuid());
            var tempFolder = tempLocation ?? Environment.GetEnvironmentVariable(TempFolderEnvironmentVariable);
            Debug.Assert(tempFolder != null);

            fileName = Path.Combine(tempFolder, fileName);
            fileName = Path.GetFullPath(fileName);

            File.WriteAllText(fileName, xml);

            return System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(
                new System.Configuration.ExeConfigurationFileMap { ExeConfigFilename = fileName },
                System.Configuration.ConfigurationUserLevel.None);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetCombinedTokens(
            IEnumerable<KeyValuePair<string, TokensConfigurationElement>> globalTokens,
            IDictionary<string, TokensConfigurationElement> applicationTokens)
        {
            var combinedTokens = globalTokens
                .Select(x => new KeyValuePair<string, string>(x.Key, x.Value.Value))
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var key in applicationTokens.Keys)
                combinedTokens[key] = applicationTokens[key].Value;

            return combinedTokens;
        }

        private static IDictionary<string, object> GetCombinedSettings(
            IEnumerable<KeyValuePair<string, SettingsConfigurationElement>> globalSettings,
            IDictionary<string, SettingsConfigurationElement> applicationSettings)
        {
            var combinedSettings = globalSettings
                .Select(x => new KeyValuePair<string, object>(x.Key, x.Value.GetObject()))
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var key in applicationSettings.Keys)
                combinedSettings[key] = applicationSettings[key].GetObject();

            return combinedSettings;
        }

        private static string GetTokenifiedXml(string untokenifiedXml, IEnumerable<KeyValuePair<string, string>> tokens)
        {
            return tokens.Aggregate(untokenifiedXml, (current, pair) => current.Replace("{" + pair.Key + "}", pair.Value));
        }

        private static IDictionary<string, object> ProcessConfiguration(System.Configuration.Configuration configuration, string applicationName)
        {
            var section = configuration.GetSection(ConfigSectionName) as ApplicationSettingsConfigurationSection;

            Debug.Assert(section != null);

            var globalSettings = section.ApplicationSettings.Applications[string.Empty];
            var applicationSettings = section.ApplicationSettings.Applications[applicationName];

            return GetCombinedSettings(globalSettings.Settings.Settings, applicationSettings.Settings.Settings);
        }

        #endregion
    }

    #endregion
}