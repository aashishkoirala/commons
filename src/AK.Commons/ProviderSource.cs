/*******************************************************************************************************************************
 * AK.Commons.ProviderSource
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
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

using AK.Commons.Composition;
using AK.Commons.Configuration;

#endregion

namespace AK.Commons
{
    #region IProviderSource

    /// <summary>
    /// Represents something that can give you a named or a default instance of a provider
    /// (a provider being anything pluggable that satisfies a contract).
    /// </summary>
    /// <typeparam name="TProvider">Provider contract type.</typeparam>
    /// <author>Aashish Koirala</author>
    public interface IProviderSource<out TProvider>
    {
        /// <summary>
        /// Gets an instance of the provider with the given name.
        /// </summary>
        /// <param name="name">Provider name.</param>
        /// <returns>Provider instance.</returns>
        TProvider this[string name] { get; }

        /// <summary>
        /// Gets the default provider instance.
        /// </summary>
        TProvider Default { get; }
    }

    #endregion

    #region ProviderSource

    /// <summary>
    /// Default implementation for IProviderSource.
    /// </summary>
    /// <author>Aashish Koirala</author>
    /// <typeparam name="TProvider">Provider type.</typeparam>
    internal class ProviderSource<TProvider> : IProviderSource<TProvider>
    {
        #region Fields

        private readonly string configKeyPrefixFormat;
        private readonly string configProviderNameKeyFormat;
        private readonly IAppConfig appConfig;
        private readonly IComposer composer;

        #endregion

        public ProviderSource(string configKeyPrefixFormat, string configProviderNameKeyFormat,
            IAppConfig appConfig, IComposer composer)
        {
            this.configKeyPrefixFormat = configKeyPrefixFormat;
            this.configProviderNameKeyFormat = configProviderNameKeyFormat;
            this.appConfig = appConfig;
            this.composer = composer;
        }

        public TProvider this[string name]
        {
            get { return this.GetProviderByName(name); }
        }

        public TProvider Default
        {
            get { return this[null]; }
        }

        private TProvider GetProviderByName(string name)
        {
            var configKeyPrefix = string.Format(this.configKeyPrefixFormat, name);
            var configProviderNameKey = string.Format(this.configProviderNameKeyFormat, name);
            var configProviderName = this.appConfig.Get<string>(configProviderNameKey);

            var provider = this.composer.Resolve<TProvider, IProviderMetadata>(x => x.Provider == configProviderName);

            var configurableProvider = provider as IConfigurableProvider;
            if (configurableProvider == null) return provider;

            configurableProvider.AssignConfigKeyPrefix(configKeyPrefix);
            return provider;
        }
    }

    #endregion
}