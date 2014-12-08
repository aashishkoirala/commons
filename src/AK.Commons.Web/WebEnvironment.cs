/*******************************************************************************************************************************
 * AK.Commons.Web.WebEnvironment
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Web Library (AKCWL).
 *  
 * AKCWL is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCWL is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCWL.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using AK.Commons.Web.LibraryResources;
using AK.Commons.Web.Minification;
using System.ComponentModel.Composition;
using System.Text;
using System.Web;

#endregion

namespace AK.Commons.Web
{
    /// <summary>
    /// Initializes the web environment (after the application environment has already been initialized) and provides common web
    /// application related services such as minification and library resources.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class WebEnvironment
    {
        #region Constants

        private const string ConfigKeyContentEncoding = "ak.commons.web.contentencoding";

        #endregion

        #region Fields

        /// <summary>
        /// The one and only IMinificationCache instance.
        /// </summary>
        private static IMinificationCache minificationCache;

        /// <summary>
        /// The one and only IMinificationManager instance.
        /// </summary>
        private static IMinificationManager minificationManager;

        /// <summary>
        /// The one and only IProviderSource&lt;IMinifier&gt; instance.
        /// </summary>
        private static IProviderSource<IMinifier> minifiers;

        /// <summary>
        /// The one and only ILibraryContentProvider instance.
        /// </summary>
        private static ILibraryContentProvider libraryContentProvider;

        #endregion

        #region Properties

        /// <summary>
        /// Whether the web environment is initialized.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// The global setting for content encoding to use.
        /// </summary>
        public static Encoding ContentEncoding { get; private set; }

        /// <summary>
        /// The application level list of minifiers. If possible, use an "Import"ed instance of IProviderSource of
        /// IMinifier than accessing this directly. Index what you get by provider name to get the minifier you
        /// are looking for.
        /// </summary>
        /// <exception cref="InitializationException">
        /// If you access this before calling Initialize.
        /// </exception>
        [Export(typeof (IProviderSource<IMinifier>))]
        public static IProviderSource<IMinifier> Minifiers
        {
            get
            {
                if (!AppEnvironment.IsInitialized)
                    throw new InitializationException(InitializationExceptionReason.ApplicationNotInitialized);

                return minifiers;
            }
        }

        /// <summary>
        /// The application level minification cache. If possible, use an "Import"ed instance of IMinificationCache
        /// rather than accessing this directly.
        /// </summary>
        /// <exception cref="InitializationException">
        /// If you access this before calling Initialize.
        /// </exception>
        [Export(typeof (IMinificationCache))]
        public static IMinificationCache MinificationCache
        {
            get
            {
                if (!AppEnvironment.IsInitialized)
                    throw new InitializationException(InitializationExceptionReason.ApplicationNotInitialized);

                return minificationCache;
            }
        }

        /// <summary>
        /// The application level minification manager (this is a wrapper that takes in a path or extension and
        /// automatically selects the right minifier to use). If possible, use an "Import"ed instance of
        /// IMinificationManager rather than accessing this directly.
        /// </summary>
        /// <exception cref="InitializationException">
        /// If you access this before calling Initialize.
        /// </exception>
        [Export(typeof (IMinificationManager))]
        public static IMinificationManager MinificationManager
        {
            get
            {
                if (!AppEnvironment.IsInitialized)
                    throw new InitializationException(InitializationExceptionReason.ApplicationNotInitialized);

                return minificationManager;
            }
        }

        /// <summary>
        /// The application level library content provider- that will give you minified content for library
        /// resources keyed by names. If possible, use an "Import"ed instance of ILibraryContentProvider rather
        /// than accessing this directly.
        /// </summary>
        /// <exception cref="InitializationException">
        /// If you access this before calling Initialize.
        /// </exception>
        [Export(typeof (ILibraryContentProvider))]
        public static ILibraryContentProvider LibraryContentProvider
        {
            get
            {
                if (!AppEnvironment.IsInitialized)
                    throw new InitializationException(InitializationExceptionReason.ApplicationNotInitialized);

                return libraryContentProvider;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the web environment. AppEnvironment.Initialize must be called prior to calling this.
        /// </summary>
        /// <param name="server">The Application.Server property.</param>
        public static void Initialize(HttpServerUtility server)
        {
            if (!AppEnvironment.IsInitialized)
                throw new InitializationException(InitializationExceptionReason.ApplicationNotInitialized);

            if (IsInitialized)
                throw new InitializationException(InitializationExceptionReason.AlreadyInitialized);

            var config = AppEnvironment.Config;
            var composer = AppEnvironment.Composer;

            var contentEncodingName = config.Get(ConfigKeyContentEncoding, string.Empty);
            ContentEncoding = string.IsNullOrWhiteSpace(contentEncodingName)
                                  ? Encoding.Default
                                  : Encoding.GetEncoding(contentEncodingName);

            minifiers = new MinifierProviderSource(composer);
            minificationManager = new MinificationManager(minifiers);

            minificationCache = new MinificationCache(config, minificationManager, ContentEncoding);
            minificationCache.Prime(server);

            libraryContentProvider = new LibraryContentProvider(ContentEncoding, minificationManager);

            IsInitialized = true;
        }

        #endregion
    }
}