/*******************************************************************************************************************************
 * AK.Commons.Web.LibraryResources.LibraryContentProvider
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

using AK.Commons.Web.Minification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace AK.Commons.Web.LibraryResources
{
    #region ILibraryContentProvider

    /// <summary>
    /// Provides access to minified ready-to-go content for "library resources", i.e. third-party or packaged JavaScript/CSS/font
    /// files (such as Angular, Bootstrap, etc. along with my own packaged JS, CSS etc.).
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface ILibraryContentProvider
    {
        /// <summary>
        /// Gets a list of JavaScript library resource content keyed by name.
        /// </summary>
        IDictionary<string, MinifiedContent> JavaScriptContentMap { get; }

        /// <summary>
        /// Gets a list of CSS library resource content keyed by name.
        /// </summary>
        IDictionary<string, MinifiedContent> StyleSheetContentMap { get; }

        /// <summary>
        /// Gets a list of font library resource content keyed by name.
        /// </summary>
        IDictionary<string, MinifiedContent> FontContentMap { get; }
    }

    #endregion

    #region LibraryContentProvider

    /// <summary>
    /// The default and only implementation of ILibraryContentProvider.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class LibraryContentProvider : ILibraryContentProvider
    {
        private static readonly IDictionary<string, string> javaScriptResourceMap =
            new Dictionary<string, string>
                {
                    {"ak-ui-angular", "ak-ui-angular"},
                    {"ng-resource", "angular-resource.min"},
                    {"ng-route", "angular-route.min"},
                    {"angular", "angular.min"},
                    {"bootstrap", "bootstrap.min"},
                    {"jquery", "jquery-2.1.1.min"},
                    {"signalr", "jquery.signalR-2.1.2.min"},
                    {"require", "require"},
                    {"ui-bootstrap", "ui-bootstrap-tpls-0.11.0.min"}
                };

        private static readonly IDictionary<string, string> styleSheetResourceMap =
            new Dictionary<string, string>
                {
                    {"bootstrap", "bootstrap.min"}
                };

        private readonly IMinificationManager minificationManager;
        private readonly Encoding encoding;

        public LibraryContentProvider(Encoding encoding, IMinificationManager minificationManager)
        {
            this.encoding = encoding;
            this.minificationManager = minificationManager;
            this.JavaScriptContentMap = this.CreateMap(javaScriptResourceMap, "JavaScript", "js");
            this.StyleSheetContentMap = this.CreateMap(styleSheetResourceMap, "StyleSheet", "css");
            this.FontContentMap = this.CreateFontContentMap();
        }

        public IDictionary<string, MinifiedContent> JavaScriptContentMap { get; private set; }
        public IDictionary<string, MinifiedContent> StyleSheetContentMap { get; private set; }
        public IDictionary<string, MinifiedContent> FontContentMap { get; private set; }

        private IDictionary<string, MinifiedContent> CreateMap(
            IEnumerable<KeyValuePair<string, string>> sourceMap, string folder, string extension)
        {
            var map = new Dictionary<string, MinifiedContent>();
            var assembly = this.GetType().Assembly;
            var namespaceName = this.GetType().Namespace;

            foreach (var pair in sourceMap)
            {
                var resourceName = string.Format(
                    "{0}.{1}.{2}.{3}", namespaceName, folder, pair.Value, extension);

                byte[] content;
                using (var stream = new MemoryStream())
                {
                    var resourceStream = assembly.GetManifestResourceStream(resourceName);
                    if (resourceStream == null) continue;

                    resourceStream.CopyTo(stream);
                    content = stream.ToArray();
                }

                var minifiedContent = this.minificationManager.Minify(
                    content, DateTime.UtcNow, extension, this.encoding, pair.Value.EndsWith(".min"));

                map[pair.Key] = minifiedContent;
            }

            return map;
        }

        private IDictionary<string, MinifiedContent> CreateFontContentMap()
        {
            var map = new Dictionary<string, MinifiedContent>();
            var assembly = this.GetType().Assembly;
            var namespaceName = this.GetType().Namespace;
            if (namespaceName == null) return new Dictionary<string, MinifiedContent>();

            var resourceNames = assembly.GetManifestResourceNames()
                                        .Where(x => x.StartsWith(namespaceName + ".Fonts"))
                                        .ToArray();

            foreach (var resourceName in resourceNames)
            {
                byte[] content;
                using (var stream = new MemoryStream())
                {
                    var resourceStream = assembly.GetManifestResourceStream(resourceName);
                    if (resourceStream == null) continue;

                    resourceStream.CopyTo(stream);
                    content = stream.ToArray();
                }

                var extension = Path.GetExtension(resourceName);
                if (extension == null) continue;

                var key = Path.GetFileNameWithoutExtension(resourceName);
                if (key == null) continue;

                key = extension.TrimStart('.') + "-" + key.Replace(namespaceName + ".Fonts.", string.Empty);

                var minifiedContent = this.minificationManager.Minify(
                    content, DateTime.UtcNow, extension, this.encoding, true);

                minifiedContent.MimeType = "application/octet-stream";

                map[key] = minifiedContent;
            }

            return map;
        }
    }

    #endregion
}