/*******************************************************************************************************************************
 * AK.Commons.Web.JsonUtility
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

using Newtonsoft.Json;
using System.IO;
using System.Web.Http;

#endregion

namespace AK.Commons.Web
{
    /// <summary>
    /// JSON handling utility methods. Uses JSON.NET (Newtonsoft), but uses the currently configured contract resolver
    /// in the Web API settings.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class JsonUtility
    {
        /// <summary>
        /// Serializes the given object as JSON.
        /// </summary>
        /// <param name="instance">Object to serialize.</param>
        /// <returns>JSON text.</returns>
        public static string Serialize(object instance)
        {
            var contractResolver =
                GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver;
            var jsonSerializer = new JsonSerializer {ContractResolver = contractResolver};

            string json;
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonSerializer.Serialize(jsonWriter, instance);
                json = stringWriter.ToString();
            }

            return json;
        }

        /// <summary>
        /// Deserializes the given JSON text.
        /// </summary>
        /// <typeparam name="T">Type to deserialize as.</typeparam>
        /// <param name="json">JSON text.</param>
        /// <returns>Deserialized instance.</returns>
        public static T Deserialize<T>(string json)
        {
            var contractResolver =
                GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver;
            var jsonSerializer = new JsonSerializer {ContractResolver = contractResolver};

            T instance;
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                instance = jsonSerializer.Deserialize<T>(jsonReader);
            }

            return instance;
        }
    }
}