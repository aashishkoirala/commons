/*******************************************************************************************************************************
 * AK.Commons.Configuration.Sections
 * Copyright © 2013 Aashish Koirala <http://aashishkoirala.github.io>
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

using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

#endregion

namespace AK.Commons.Configuration.Sections
{
    /// <summary>
    /// This class is used internally to read configuration settings.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class TokensConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TokensConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var tokensConfigurationElement = element as TokensConfigurationElement;
            Debug.Assert(tokensConfigurationElement != null);

            return tokensConfigurationElement.Name;
        }

        public IDictionary<string, TokensConfigurationElement> Tokens
        {
            get 
            { 
                return (
                    from object item in this
                    select item as TokensConfigurationElement)
                    .ToDictionary(x => x.Name); 
            }
        }
    }
}