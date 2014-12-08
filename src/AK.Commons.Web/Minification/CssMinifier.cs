/*******************************************************************************************************************************
 * AK.Commons.Web.Minification.CssMinifier
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

using AK.Commons.Composition;
using Microsoft.Ajax.Utilities;
using System.ComponentModel.Composition;
using System.Text;

#endregion

namespace AK.Commons.Web.Minification
{
    /// <summary>
    /// Minifies CSS content (uses AjaxMin).
    /// </summary>
    [Export(typeof (IMinifier))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [ProviderMetadata("css")]
    public class CssMinifier : IMinifier
    {
        public string Minify(string content)
        {
            var cssSettings = new CssSettings
                {
                    BlocksStartOnSameLine = BlockStart.SameLine,
                    RemoveEmptyBlocks = true,
                    ColorNames = CssColor.Strict,
                    CommentMode = CssComment.None,
                    CssType = CssType.FullStyleSheet,
                    MinifyExpressions = true,
                    IndentSize = 0,
                    OutputMode = OutputMode.SingleLine,
                    TermSemicolons = true
                };

            var minifier = new Minifier();
            return minifier.MinifyStyleSheet(content, cssSettings);
        }

        public byte[] Minify(byte[] content, Encoding encoding)
        {
            return encoding.GetBytes(this.Minify(encoding.GetString(content)));
        }
    }
}