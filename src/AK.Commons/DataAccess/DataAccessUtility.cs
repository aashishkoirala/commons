/*******************************************************************************************************************************
 * AK.Commons.DataAccess.DataAccessUtility
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

namespace AK.Commons.DataAccess
{
    /// <summary>
    /// Data access utility methods.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class DataAccessUtility
    {
        /// <summary>
        /// Executes the given action against a new unit of work created using the given factory.
        /// </summary>
        /// <param name="factory">Unit-of-work factory.</param>
        /// <param name="action">Action to execute.</param>
        public static void Execute(this IUnitOfWorkFactory factory, Action<IUnitOfWork> action)
        {
            using (var unit = factory.Create())
            {
                action(unit);
                unit.Commit();
            }
        }

        /// <summary>
        /// Executes the given action against a new unit of work created using the given factory.
        /// </summary>
        /// <typeparam name="T">Return type of action.</typeparam>
        /// <param name="factory">Unit-of-work factory.</param>
        /// <param name="action">Action to execute.</param>
        /// <returns>Result of action.</returns>
        public static T Execute<T>(this IUnitOfWorkFactory factory, Func<IUnitOfWork, T> action)
        {
            using (var unit = factory.Create())
            {
                var result = action(unit);
                unit.Commit();
                return result;
            }
        }
    }
}