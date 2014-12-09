/*******************************************************************************************************************************
 * AK.Commons.Web.Minification.MinificationContext
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

using System;
using System.IO;
using System.Text;
using System.Web;

#endregion

namespace AK.Commons.Web.Minification
{
    /// <summary>
    /// Handles minification on a single web request. Takes care of dealing with the response as well.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class MinificationContext
    {
        #region Fields

        private readonly HttpRequest request;
        private readonly HttpResponse response;
        private readonly HttpServerUtility server;
        private readonly Encoding encoding;
        private readonly bool supportBrowserCaching;
        private readonly IMinificationCache minificationCache;

        #endregion

        #region Constructor

        public MinificationContext(
            HttpContext context,
            Encoding encoding,
            bool supportBrowserCaching,
            IMinificationCache minificationCache)
        {
            this.request = context.Request;
            this.response = context.Response;
            this.server = context.Server;
            this.encoding = encoding;
            this.supportBrowserCaching = supportBrowserCaching;
            this.minificationCache = minificationCache;
        }

        #endregion

        #region Methods

        public void Process()
        {
            var url = this.request.Url.RemoveQueryString();
            var path = this.server.MapPath(url.PathAndQuery);

            if (!File.Exists(path))
            {
                this.RespondNotFound();
                return;
            }

            if (this.supportBrowserCaching && this.IsBrowserCacheCurrent(path))
            {
                this.response.StatusCode = 304;
                this.response.Flush();
                this.response.End();
                return;
            }

            var contentEntry = this.minificationCache.Get(path);
            if (contentEntry == null)
            {
                this.RespondNotFound();
                return;
            }

            this.response.ContentEncoding = encoding;
            this.response.ContentType = contentEntry.MimeType;

            var data = this.CompressIfSupported(contentEntry);

            this.response.OutputStream.Write(data, 0, data.Length);
            if (this.supportBrowserCaching)
            {
                this.response.Cache.SetCacheability(HttpCacheability.Private);
                this.response.Cache.SetLastModified(contentEntry.LastModified);
            }
            else
            {
                this.response.Cache.SetCacheability(HttpCacheability.NoCache);
                this.response.Cache.SetNoStore();
                this.response.Cache.SetExpires(DateTime.Now.AddDays(-1));
                this.response.Cache.SetLastModified(DateTime.Now.AddDays(1));
            }
            this.response.Flush();
        }

        private void RespondNotFound()
        {
            this.response.StatusCode = 404;
            this.response.Flush();
            this.response.End();
        }

        private bool IsBrowserCacheCurrent(string path)
        {
            var ifModifiedSinceHeader = this.request.Headers["If-Modified-Since"];
            if (string.IsNullOrWhiteSpace(ifModifiedSinceHeader)) return false;

            DateTime ifModifiedSince;
            if (!DateTime.TryParse(ifModifiedSinceHeader, out ifModifiedSince))
                return false;

            var ifModifiedSinceTicks = ifModifiedSince.ToUniversalTime().Ticks;
            var lastModifiedTicks = File.GetLastWriteTimeUtc(path).Ticks;

            if (lastModifiedTicks <= ifModifiedSinceTicks) return true;

            var seconds = (lastModifiedTicks - ifModifiedSinceTicks)/TimeSpan.TicksPerSecond;
            return seconds < 10;
        }

        private byte[] CompressIfSupported(MinifiedContent entry)
        {
            var acceptEncoding = this.request.Headers["Accept-Encoding"];
            if (string.IsNullOrWhiteSpace(acceptEncoding)) return entry.UncompressedContent;

            if (acceptEncoding.Contains("gzip"))
            {
                this.response.AppendHeader("Content-Encoding", "gzip");
                return entry.GZipCompressedContent;
            }

            if (acceptEncoding.Contains("deflate"))
            {
                this.response.AppendHeader("Content-Encoding", "deflate");
                return entry.DeflateCompressedContent;
            }
            return entry.UncompressedContent;
        }

        #endregion
    }
}