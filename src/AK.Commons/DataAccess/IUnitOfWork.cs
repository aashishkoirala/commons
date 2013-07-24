/*******************************************************************************************************************************
 * AK.Commons.DataAccess.IUnitOfWork
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
using System.Linq;

#endregion

namespace AK.Commons.DataAccess
{
    /// <summary>
    /// Represents a unit-of-work in the unit-of-work pattern.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Whether the unit-of-work is still valid. Should be true when the unit is created,
        /// and set to false after a commit/rollback or when the unit is disposed.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets the given entity by key.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <typeparam name="TKey">Key type.</typeparam>
        /// <param name="key">Key value.</param>
        /// <returns>Entity instance.</returns>
        TEntity Get<TEntity, TKey>(TKey key);

        /// <summary>
        /// Gets a queryable that can be used to query off the entity set.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>Queryable of entity.</returns>
        IQueryable<TEntity> Query<TEntity>();

        /// <summary>
        /// Runs a query that is built using the provided query builder.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <typeparam name="TResult">Type of result of the query.</typeparam>
        /// <param name="queryBuilder">Query builder function.</param>
        /// <returns>Query result.</returns>
        TResult Query<TEntity, TResult>(Func<IQueryable<TEntity>, TResult> queryBuilder);

        /// <summary>
        /// Saves the given entity.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="entity">Entity instance.</param>
        void Save<TEntity>(TEntity entity);

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <param name="entity">Entity instance.</param>
        void Delete<TEntity>(TEntity entity);

        /// <summary>
        /// Commits the unit of work. Should set IsValid to false.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rolls the unit of work back. Should set IsValid to false.
        /// </summary>
        void Rollback();
    }
}