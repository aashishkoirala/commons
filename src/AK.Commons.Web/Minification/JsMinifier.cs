/*******************************************************************************************************************************
 * AK.Commons.Web.Minification.JsMinifier
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
    /// Minifies JS content (uses AjaxMin).
    /// </summary>
    [Export(typeof (IMinifier))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [ProviderMetadata("js")]
    public class JsMinifier : IMinifier
    {
        public string Minify(string content)
        {
            var codeSettings = new CodeSettings
                {
                    AmdSupport = true,
                    BlocksStartOnSameLine = BlockStart.SameLine,
                    MinifyCode = true,
                    RemoveUnneededCode = true,
                    StripDebugStatements = true,
                    IndentSize = 0,
                    OutputMode = OutputMode.SingleLine,
                    TermSemicolons = true
                };

            var minifier = new Minifier();
            return minifier.MinifyJavaScript(content, codeSettings);
        }

        public byte[] Minify(byte[] content, Encoding encoding)
        {
            return encoding.GetBytes(this.Minify(encoding.GetString(content)));
        }
    }
}