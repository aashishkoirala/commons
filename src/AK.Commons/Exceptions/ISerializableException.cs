/*******************************************************************************************************************************
 * AK.Commons.Exceptions.GeneralException
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

namespace AK.Commons.Exceptions
{
    /// <summary>
    /// Represents an exception with a SerializedData property.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface ISerializableException
    {
        /// <summary>
        /// Custom serialized data.
        /// </summary>
        string SerializedData { get; }
    }
}