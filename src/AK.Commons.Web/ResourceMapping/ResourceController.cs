/*******************************************************************************************************************************
 * AK.Commons.Web.ResourceMapping.ResourceController
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

using AK.Commons.Web.Filters;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

#endregion

namespace AK.Commons.Web.ResourceMapping
{
    /// <summary>
    /// Generic REST controller. Map such that first parameter corresponds to the "resource" parameter. This controller will
    /// then serve requests based on resource mapping configuration defined using ResourceMap.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [WebApiLogExceptionFilter]
    public class ResourceController : ApiController
    {
        private readonly IResourceProvider resourceProvider;

        public ResourceController()
        {
            this.resourceProvider = ResourceMapConfiguration.Provider;
        }

        public HttpResponseMessage Get(string resource, string id)
        {
            return this.resourceProvider == null
                       ? new HttpResponseMessage(HttpStatusCode.NotFound)
                       : this.resourceProvider.GetResponse(resource, HttpMethod.Get, false, id, this.ControllerContext);
        }

        public HttpResponseMessage Get(string resource)
        {
            if (this.resourceProvider == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            var queryMap = this.Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
            var queryJson = JsonUtility.Serialize(queryMap);

            return this.resourceProvider.GetResponse(resource, HttpMethod.Get, true, queryJson, this.ControllerContext);
        }

        public async Task<HttpResponseMessage> Post(string resource)
        {
            if (this.resourceProvider == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            var input = await this.Request.Content.ReadAsStringAsync();

            var isList = false;
            if (!string.IsNullOrWhiteSpace(input))
            {
                var token = JToken.Parse(input);
                isList = token is JArray;
            }

            return this.resourceProvider.GetResponse(resource, HttpMethod.Post, isList, input, this.ControllerContext);
        }

        public async Task<HttpResponseMessage> Put(string resource)
        {
            if (this.resourceProvider == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            var input = await this.Request.Content.ReadAsStringAsync();

            var isList = false;
            if (!string.IsNullOrWhiteSpace(input))
            {
                var token = JToken.Parse(input);
                isList = token is JArray;
            }

            return this.resourceProvider.GetResponse(resource, HttpMethod.Put, isList, input, this.ControllerContext);
        }

        public HttpResponseMessage Delete(string resource, string id)
        {
            return this.resourceProvider == null
                       ? new HttpResponseMessage(HttpStatusCode.NotFound)
                       : this.resourceProvider.GetResponse(resource, HttpMethod.Delete, false, id,
                                                           this.ControllerContext);
        }

        public async Task<HttpResponseMessage> Delete(string resource)
        {
            if (this.resourceProvider == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            var input = await this.Request.Content.ReadAsStringAsync();

            return this.resourceProvider.GetResponse(resource, HttpMethod.Delete, true, input, this.ControllerContext);
        }
    }
}