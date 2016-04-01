/*******************************************************************************************************************************
 * AK.Commons.DataAccess.IRepository
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

using System.Linq;

namespace AK.Commons.DataAccess
{
    /// <summary>
    /// Represents a generic repository.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IRepository
    {
    }

    /// <summary>
    /// Represents a repository for the given type.
    /// </summary>
    /// <typeparam name="T">Type of entity this repository houses.</typeparam>
    /// <author>Aashish Koirala</author>
    public interface IRepository<T> : IRepository where T : class
    {
        T Create<TKey>(TKey id) where TKey : struct;

        T Get<TKey>(TKey id) where TKey : struct;

        T GetOrCreate<TKey>(TKey id) where TKey : struct;
        
        /// <summary>
        /// A LINQ-queryable interface for data access to the repository.
        /// </summary>
        IQueryable<T> Query { get; }

        /// <summary>
        /// Saves the given entity to the repository.
        /// </summary>
        /// <param name="thing">Entity to save.</param>
        void Save(T thing);

        /// <summary>
        /// Deletes the given entity from the repository.
        /// </summary>
        /// <param name="thing">Entity to delete.</param>
        void Delete(T thing);
    }
}