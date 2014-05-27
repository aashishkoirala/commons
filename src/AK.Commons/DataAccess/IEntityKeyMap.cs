/*******************************************************************************************************************************
 * AK.Commons.DataAccess.IEntityKeyMap
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

using System;
using System.Linq.Expressions;

#endregion

namespace AK.Commons.DataAccess
{
    /// <summary>
    /// Implement this to provide a entity-key map store that consists of information
    /// on entities and their keys.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IEntityKeyMap
    {
        /// <summary>
        /// Maps the given entity to its key property.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <param name="expression">Key expression.</param>
        void Map<TEntity, TKey>(Expression<Func<TEntity, TKey>> expression);
    }
}