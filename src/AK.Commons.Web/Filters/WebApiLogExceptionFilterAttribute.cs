/*******************************************************************************************************************************
 * AK.Commons.Web.Filters.WebApiLogExceptionFilterAttribute
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

using AK.Commons.Logging;
using System.Web.Http.Filters;

#endregion

namespace AK.Commons.Web.Filters
{
    /// <summary>
    /// Exception filter for Web API that logs exceptions.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class WebApiLogExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// The logger to use - if not set, the default environment logger is used, which is the desired behavior.
        /// This property is here mostly for you to inject the logger for testing.
        /// </summary>
        public IAppLogger Logger { get; set; }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var logger = this.Logger ?? AppEnvironment.Logger;
            logger.Error(actionExecutedContext.Exception);

            base.OnException(actionExecutedContext);
        }
    }
}