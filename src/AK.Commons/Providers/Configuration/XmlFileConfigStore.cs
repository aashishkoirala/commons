/*******************************************************************************************************************************
 * AK.Commons.Providers.Configuration.XmlFileConfigStore
 * Copyright © 2013-2014 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Library (AKCL).
 *  
 * AKCL is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCL is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCL.  If not, see <http://www.gnu.org/licenses/>.
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using System.Reflection;
using System.Web;
using AK.Commons.Configuration;
using System.IO;

#endregion

namespace AK.Commons.Providers.Configuration
{
    /// <summary>
    /// Configuration store based on an XML file.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class XmlFileConfigStore : IConfigStore
    {
        #region Methods (IConfigStore)

        /// <summary>
        /// The XML file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets the configuration XML.
        /// </summary>
        /// <returns>Configuration XML</returns>
        public string GetConfigurationXml()
        {
            var entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var entryAssemblyDirectory = (Path.GetDirectoryName(entryAssembly.Location) ?? string.Empty).ToLower();

            var filePath = this.FilePath;
            ManageWebRelativePath(ref filePath);
            ManageRelativePath(entryAssemblyDirectory, ref filePath);

            return File.ReadAllText(filePath);
        }

        #endregion

        #region Methods (Private)

        private static void ManageWebRelativePath(ref string filePath)
        {
            if (!filePath.StartsWith("~/")) return;
            if (HttpContext.Current == null) return;

            filePath = HttpContext.Current.Server.MapPath(filePath);
        }

        private static void ManageRelativePath(string entryAssemblyDirectory, ref string filePath)
        {
            if (Path.IsPathRooted(filePath)) return;

            filePath = Path.GetFullPath(Path.Combine(entryAssemblyDirectory, filePath));
        }

        #endregion
    }
}