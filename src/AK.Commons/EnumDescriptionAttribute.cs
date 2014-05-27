/*******************************************************************************************************************************
 * AK.Commons.EnumDescriptionAttribute
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

using System;

namespace AK.Commons
{
    /// <summary>
    /// This attribute can be applied to Enum members to include human-readable descriptions
    /// that are then accessed using Enum.Describe().
    /// </summary>
    /// <author>Aashish Koirala</author>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class EnumDescriptionAttribute : Attribute
    {
        /// <summary>
        /// The human-readable description for the Enum member.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Creates a new instance of the attribute.
        /// </summary>
        /// <param name="description">The human-readable description for the Enum member.</param>
        public EnumDescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}