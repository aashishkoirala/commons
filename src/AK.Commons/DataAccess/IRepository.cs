/*******************************************************************************************************************************
 * AK.Commons.DataAccess.IRepository
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

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace AK.Commons.DataAccess
{
    /// <summary>
    /// Represents a generic repository that can be attached to a IUnitOfWork instance.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IRepository
    {
        /// <summary>
        /// Gets or sets the unit-of-work instance.
        /// </summary>
        IUnitOfWork UnitOfWork { get; set; }
    }

    /// <summary>
    /// Represents a entity-specific repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TKey">Entity key type.</typeparam>
    public interface IRepository<TEntity, in TKey> : IRepository
    {
        /// <summary>
        /// Gets the entity by key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Entity.</returns>
        TEntity Get(TKey key);
        
        /// <summary>
        /// Gets a list of all entities.
        /// </summary>
        /// <returns>List of all entities.</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Executes the given query and returns the specified type of result.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="queryBuilder">Query builder function.</param>
        /// <returns>Result object.</returns>
        TResult GetFor<TResult>(Func<IQueryable<TEntity>, TResult> queryBuilder);

        /// <summary>
        /// Executes the given query that returns a list of the entity.
        /// </summary>
        /// <param name="queryBuilder">Query builder function that returns a list of the entity.</param>
        /// <returns>List of entities.</returns>
        IEnumerable<TEntity> GetList(Func<IQueryable<TEntity>, IEnumerable<TEntity>> queryBuilder);

        /// <summary>
        /// Saves the given entity to the database.
        /// </summary>
        /// <param name="entity">Entity object.</param>
        void Save(TEntity entity);

        /// <summary>
        /// Replaces the list of entities in the database with the given list of entities.
        /// </summary>
        /// <param name="entityList">List of entities.</param>
        /// <param name="equalityComparer">Predicate that defines how to tell if two entities are the same row.</param>
        void Replace(IEnumerable<TEntity> entityList, Func<TEntity, TEntity, bool> equalityComparer);

        /// <summary>
        /// Deletes the given entity from the database.
        /// </summary>
        /// <param name="entity">Entity object.</param>
        void Delete(TEntity entity);
    }
}