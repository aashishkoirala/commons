/*******************************************************************************************************************************
 * AK.Commons.Web.ResourceMapping.ResponseMessageUtility
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

using AK.Commons.Services;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

#endregion

namespace AK.Commons.Web.ResourceMapping
{
    /// <summary>
    /// Creates HTTP response messages from various types of responses.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal static class ResponseMessageUtility
    {
        private const string KeyHeader = "X-Key";
        private const string ErrorCodeHeader = "X-ErrorCode";

        public static HttpResponseMessage GetResponseMessage(
            this OperationResult response,
            IResourceProvider resourceProvider, HttpMethod method,
            HttpControllerContext context)
        {
            return response.IsSuccess
                       ? context.Request.CreateResponse(method == HttpMethod.Post
                                                            ? HttpStatusCode.Created
                                                            : HttpStatusCode.OK)
                       : GetErrorResponse(response, resourceProvider, context);
        }

        public static HttpResponseMessage GetResponseMessage<T>(
            this OperationResult<T> response,
            IResourceProvider resourceProvider, HttpMethod method,
            HttpControllerContext context)
        {
            return response.IsSuccess
                       ? context.Request.CreateResponse(method == HttpMethod.Post
                                                            ? HttpStatusCode.Created
                                                            : HttpStatusCode.OK, response.Result)
                       : GetErrorResponse(response, resourceProvider, context);
        }

        public static HttpResponseMessage GetResponseMessage<T>(
            this OperationResults<T> response,
            HttpMethod method, HttpControllerContext context)
        {
            if (response.IsSuccess)
            {
                var statusCode = method == HttpMethod.Post ? HttpStatusCode.Created : HttpStatusCode.OK;
                return context.Request.CreateResponse(statusCode, response.Results.Select(x => x.Result).ToArray());
            }

            var successfulResults = response.Results
                                            .Where(x => x.IsSuccess)
                                            .Select(x => x.Result)
                                            .ToArray();

            var errors = response.Results
                                 .Where(x => !x.IsSuccess)
                                 .Select(x => new { x.ErrorCode, x.Key, x.Message })
                                 .ToArray();

            var reason = string.Format("Only {0} of {1} operations were successful.",
                                       successfulResults.Length, response.Results.Count);

            var content = JsonUtility.Serialize(new
                {
                    SuccessfulResults = successfulResults,
                    Errors = errors
                });

            var message = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, reason);

            message.ReasonPhrase = reason;
            message.Content = new StringContent(content);
            message.Headers.Add(KeyHeader, errors.Select(x => x.Key));
            message.Headers.Add(ErrorCodeHeader, errors.Select(x => x.ErrorCode));

            return message;
        }

        public static HttpResponseMessage GetResponseMessage(
            this OperationResults response,
            HttpMethod method, HttpControllerContext context)
        {
            if (response.IsSuccess)
            {
                var statusCode = method == HttpMethod.Post ? HttpStatusCode.Created : HttpStatusCode.OK;
                return context.Request.CreateResponse(statusCode);
            }

            var errors = response.Results
                                 .Where(x => !x.IsSuccess)
                                 .Select(x => new { x.ErrorCode, x.Key, x.Message })
                                 .ToArray();

            var reason = string.Format("Only {0} of {1} operations were successful.",
                                       response.Results.Count - errors.Length, response.Results.Count);

            var content = JsonUtility.Serialize(new {Errors = errors});

            var message = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, reason);

            message.ReasonPhrase = reason;
            message.Content = new StringContent(content);
            message.Headers.Add(KeyHeader, errors.Select(x => x.Key));
            message.Headers.Add(ErrorCodeHeader, errors.Select(x => x.ErrorCode));

            return message;
        }

        private static HttpResponseMessage GetErrorResponse(
            OperationResult response, IResourceProvider resourceProvider, HttpControllerContext context)
        {
            var statusCode = resourceProvider.GetStatusCode(response.ErrorCode);
            var message = context.Request.CreateErrorResponse(statusCode, response.Message);

            message.ReasonPhrase = response.Message;
            message.Headers.Add(KeyHeader, response.Key);
            message.Headers.Add(ErrorCodeHeader, response.ErrorCode);

            return message;
        }
    }
}