/*******************************************************************************************************************************
 * AK.Commons.Web.Minification.MinificationCache
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

using AK.Commons.Configuration;
using AK.Commons.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

#endregion

namespace AK.Commons.Web.Minification
{
    #region IMinificationCache

    /// <summary>
    /// Provides caching services for minified static content.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IMinificationCache
    {
        /// <summary>
        /// Gets the minified version of the given path from the cache.
        /// </summary>
        /// <param name="path">File path corresponding to content required.</param>
        /// <returns>Minified content from cache.</returns>
        MinifiedContent Get(string path);

        /// <summary>
        /// Primes the cache.
        /// </summary>
        /// <param name="server">The web application's Server property.</param>
        void Prime(HttpServerUtility server);
    }

    #endregion

    #region MinificationCache

    /// <summary>
    /// The one and only implementatin of IMinificationCache.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class MinificationCache : IMinificationCache
    {
        #region Constants

        private const string ConfigKeyPrime = "ak.commons.web.minificationcache.prime";
        private const string ConfigKeyLocked = "ak.commons.web.minificationcache.locked";
        private const string ConfigKeyPrimePaths = "ak.commons.web.minificationcache.primepaths";
        private const string ConfigKeyPrimeExtensions = "ak.commons.web.minificationcache.primeextensions";
        private const string ConfigKeyPrimeExtensionsDefault = ".html;.css;.js";

        #endregion

        #region Fields

        private readonly IDictionary<string, MinifiedContent> innerContentMap =
            new Dictionary<string, MinifiedContent>();

        private readonly LockedObject<IDictionary<string, MinifiedContent>> contentMap;
        private readonly Encoding encoding;

        private readonly IMinificationManager minificationManager;
        private readonly bool prime;
        private readonly bool locked;
        private readonly string primePaths;
        private readonly string primeExtensions;

        #endregion

        #region Constructor

        public MinificationCache(IAppConfig config, IMinificationManager minificationManager, Encoding encoding)
        {
            this.contentMap = new LockedObject<IDictionary<string, MinifiedContent>>(innerContentMap);
            this.minificationManager = minificationManager;
            this.encoding = encoding;

            this.prime = config.Get(ConfigKeyPrime, false);
            this.locked = config.Get(ConfigKeyLocked, false);
            this.primePaths = config.Get(ConfigKeyPrimePaths, string.Empty);
            this.primeExtensions = config.Get(ConfigKeyPrimeExtensions, ConfigKeyPrimeExtensionsDefault);
        }

        #endregion

        #region Methods (IMinificationCache)

        public MinifiedContent Get(string path)
        {
            var key = path.ToLower();

            if (this.locked) return this.innerContentMap[key];

            var lastModified = File.GetLastWriteTimeUtc(path);

            // ReSharper disable ImplicitlyCapturedClosure

            var minifiedContent = this.contentMap.ExecuteRead(map => map.LookFor(key).ValueOrDefault);
            if (minifiedContent != null && lastModified <= minifiedContent.LastModified)
            {
                minifiedContent = null;
            }

            // ReSharper restore ImplicitlyCapturedClosure

            if (minifiedContent == null)
            {
                this.contentMap.ExecuteWrite(map =>
                    {
                        minifiedContent = map.LookFor(key).ValueOrDefault;
                        if (minifiedContent != null && lastModified <= minifiedContent.LastModified)
                        {
                            minifiedContent = null;
                        }

                        if (minifiedContent != null) return;

                        minifiedContent = this.minificationManager.Minify(path, this.encoding);

                        map[key] = minifiedContent;
                    });
            }

            return minifiedContent;
        }

        public void Prime(HttpServerUtility server)
        {
            if (!this.prime) return;
            if (string.IsNullOrWhiteSpace(this.primePaths)) return;

            var paths = this.primePaths
                            .Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries)
                            .Select(server.MapPath)
                            .Distinct()
                            .ToArray();

            var extensions = this.primeExtensions.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

            var files = paths
                .Select(x => Directory.GetFiles(x, "*.*", SearchOption.AllDirectories))
                .SelectMany(x => x)
                .Where(x => extensions.Any(x.EndsWith))
                .Distinct()
                .ToArray();

            this.contentMap.ExecuteWrite(map =>
                {
                    foreach (var file in files)
                    {
                        map[file.ToLower()] = this.minificationManager.Minify(file, this.encoding);
                    }
                });
        }

        #endregion
    }

    #endregion
}