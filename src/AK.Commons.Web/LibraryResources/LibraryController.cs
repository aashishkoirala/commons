/*******************************************************************************************************************************
 * AK.Commons.Web.LibraryResources.LibraryController
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
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

#endregion

namespace AK.Commons.Web.LibraryResources
{
    /// <summary>
    /// MVC controller that servers up library-resource content. To enable, use RouteTable.Routes.MapLibraryContent().
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class LibraryController : Controller
    {
        #region Constants/Fields

        private const string Css = "css";
        private const string Js = "js";
        private const string Font = "font";
        private const string AcceptEncodingHeader = "Accept-Encoding";
        private const string ContentEncodingHeader = "Content-Encoding";
        private const string GZipHeaderValue = "gzip";
        private const string DeflateHeaderValue = "deflate";

        private readonly ILibraryContentProvider libraryContentProvider = WebEnvironment.LibraryContentProvider;

        #endregion

        /// <summary>
        /// MVC action that serves up the given library-resource content.
        /// </summary>
        /// <param name="type">One of: "js", "css" or "font".</param>
        /// <param name="name">Name of resource.</param>
        /// <returns>Library-resource content as requested.</returns>
        public ActionResult Library(string type, string name)
        {
            IDictionary<string, MinifiedContent> map = null;

            switch (type)
            {
                case Js:
                    map = this.libraryContentProvider.JavaScriptContentMap;
                    break;
                case Css:
                    map = this.libraryContentProvider.StyleSheetContentMap;
                    break;
                case Font:
                    map = this.libraryContentProvider.FontContentMap;
                    break;
            }

            if (map == null) return new HttpNotFoundResult();

            var minifiedContent = map.LookFor(name).ValueOrDefault;
            if (minifiedContent == null) return new HttpNotFoundResult();

            var data = this.CompressIfSupported(minifiedContent);

            this.Response.ContentEncoding = WebEnvironment.ContentEncoding;
            this.Response.ContentType = minifiedContent.MimeType;

            this.Response.OutputStream.Write(data, 0, data.Length);
            this.Response.Cache.SetCacheability(HttpCacheability.Private);
            this.Response.Cache.SetLastModified(minifiedContent.LastModified);
            this.Response.Flush();

            return new EmptyResult();
        }

        private byte[] CompressIfSupported(MinifiedContent entry)
        {
            var acceptEncoding = this.Request.Headers[AcceptEncodingHeader];
            if (string.IsNullOrWhiteSpace(acceptEncoding)) return entry.UncompressedContent;

            if (acceptEncoding.Contains(GZipHeaderValue))
            {
                this.Response.AppendHeader(ContentEncodingHeader, GZipHeaderValue);
                return entry.GZipCompressedContent;
            }

            if (acceptEncoding.Contains(DeflateHeaderValue))
            {
                this.Response.AppendHeader(ContentEncodingHeader, DeflateHeaderValue);
                return entry.DeflateCompressedContent;
            }

            return entry.UncompressedContent;
        }
    }
}