/*******************************************************************************************************************************
 * AK.Commons.Web.Minification.MinifiedContent
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

using System;

namespace AK.Commons.Web.Minification
{
    /// <summary>
    /// Represents minified and possibly compressed content.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class MinifiedContent
    {
        /// <summary>
        /// Minified, uncompressed content.
        /// </summary>
        public byte[] UncompressedContent { get; set; }

        /// <summary>
        /// Minified, GZIP-compressed content.
        /// </summary>
        public byte[] GZipCompressedContent { get; set; }

        /// <summary>
        /// Minified, Deflate-compressed content.
        /// </summary>
        public byte[] DeflateCompressedContent { get; set; }

        /// <summary>
        /// MIME type.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Last modification date of content.
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}