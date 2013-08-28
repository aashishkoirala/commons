/*******************************************************************************************************************************
 * AK.Commons.DataAccess.IEntityKeyMapper
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

namespace AK.Commons.DataAccess
{
    /// <summary>
    /// Implement this interface to put in logic to map entities to their keys.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IEntityKeyMapper
    {
        /// <summary>
        /// This is where all the mappings against the given IEntityKeyMap should be done.
        /// </summary>
        /// <param name="map">IEntityKeyMap instance.</param>
        void Map(IEntityKeyMap map);
    }
}