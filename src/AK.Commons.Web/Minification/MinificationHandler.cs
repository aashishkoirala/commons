/*******************************************************************************************************************************
 * AK.Commons.Web.Minification.MinificationHandler
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

using System.Web;

namespace AK.Commons.Web.Minification
{
    /// <summary>
    /// HTTP request handler for static content minification. Register this for .css, .html and .js.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class MinificationHandler : IHttpHandler
    {
        private const string ConfigKeySupportBrowserCaching = "ak.commons.web.minification.supportbrowsercaching";

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var supportBrowserCaching = AppEnvironment.Config.Get(ConfigKeySupportBrowserCaching, false);
            var minificationContext = new MinificationContext(
                context, WebEnvironment.ContentEncoding, supportBrowserCaching, WebEnvironment.MinificationCache);

            minificationContext.Process();
        }
    }
}