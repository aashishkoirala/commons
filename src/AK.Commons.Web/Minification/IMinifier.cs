/*******************************************************************************************************************************
 * AK.Commons.Web.Minification.IMinifier
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

using System.Text;

namespace AK.Commons.Web.Minification
{
    /// <summary>
    /// Represents a static content minifier.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IMinifier
    {
        /// <summary>
        /// Minifies the given content.
        /// </summary>
        /// <param name="content">Content to minify.</param>
        /// <returns>Minified content.</returns>
        string Minify(string content);

        /// <summary>
        /// Minifies the given content.
        /// </summary>
        /// <param name="content">Content to minify.</param>
        /// <param name="encoding">Encoding to use for string conversion.</param>
        /// <returns>Minified content.</returns>
        byte[] Minify(byte[] content, Encoding encoding);
    }
}