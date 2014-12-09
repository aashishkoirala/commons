/*******************************************************************************************************************************
 * AK.Commons.Web.Minification.MinificationManager
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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

#endregion

namespace AK.Commons.Web.Minification
{
    #region IMinificationManager

    /// <summary>
    /// Handles minification given a path or content with type information. Automatically selects the right minifier to use.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IMinificationManager
    {
        /// <summary>
        /// Minifies the contents of the given file path.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="encoding">Encoding to use for string conversion.</param>
        /// <returns>Minified content.</returns>
        MinifiedContent Minify(string path, Encoding encoding);

        /// <summary>
        /// Minifies the given content.
        /// </summary>
        /// <param name="content">Content to minify.</param>
        /// <param name="lastModified">Last modification of content.</param>
        /// <param name="extension">Extension to use to select minifier.</param>
        /// <param name="encoding">Encoding to use for string conversion.</param>
        /// <param name="isAlreadyMinified">
        /// Is the content already minified? If so, skips minification and just does the compression.
        /// </param>
        /// <returns>Minified content.</returns>
        MinifiedContent Minify(byte[] content, DateTime lastModified, string extension, Encoding encoding,
                               bool isAlreadyMinified);
    }

    #endregion

    #region MinificationManager

    /// <summary>
    /// The one and only implementation of IMinificationManager.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class MinificationManager : IMinificationManager
    {
        #region Fields

        private static readonly IDictionary<string, string> mimeTypeMap = new Dictionary<string, string>
            {
                {"html", "text/html"},
                {"css", "text/css"},
                {"js", "application/x-javascript"}
            };

        private readonly IProviderSource<IMinifier> minifierSource;

        #endregion

        public MinificationManager(IProviderSource<IMinifier> minifierSource)
        {
            this.minifierSource = minifierSource;
        }

        public MinifiedContent Minify(string path, Encoding encoding)
        {
            return this.Minify(
                File.ReadAllBytes(path),
                File.GetLastWriteTimeUtc(path),
                Path.GetExtension(path),
                encoding,
                path.Contains(".min."));
        }

        public MinifiedContent Minify(byte[] content, DateTime lastModified, string extension, Encoding encoding,
                                      bool isAlreadyMinified)
        {
            var minifiedContent = new MinifiedContent
                {
                    UncompressedContent = content,
                    LastModified = lastModified
                };

            if (extension == null)
            {
                ManageCompression(minifiedContent);
                return minifiedContent;
            }

            extension = extension.TrimStart('.').ToLower();

            var mimeType = mimeTypeMap.LookFor(extension);
            if (!mimeType.IsThere)
            {
                ManageCompression(minifiedContent);
                return minifiedContent;
            }

            minifiedContent.MimeType = mimeType.Value;

            if (!isAlreadyMinified)
                minifiedContent.UncompressedContent = this.minifierSource[extension].Minify(content, encoding);

            ManageCompression(minifiedContent);

            return minifiedContent;
        }

        private static void ManageCompression(MinifiedContent content)
        {
            using (var stream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    gzipStream.Write(content.UncompressedContent, 0,
                                     content.UncompressedContent.Length);
                }
                content.GZipCompressedContent = stream.ToArray();
            }

            using (var stream = new MemoryStream())
            {
                using (var deflateStream = new DeflateStream(stream, CompressionMode.Compress))
                {
                    deflateStream.Write(content.UncompressedContent, 0,
                                        content.UncompressedContent.Length);
                }
                content.DeflateCompressedContent = stream.ToArray();
            }
        }
    }

    #endregion
}