/*******************************************************************************************************************************
 * AK.Commons.DataAccess.RepositoryBase
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
    /// Provides common functionality across IRepository implementations. If possible,
    /// inherit from this class to implement repositories rather than directly implementing
    /// IRepository.
    /// </summary>
    /// <typeparam name="TEntity">See IRepository.</typeparam>
    /// <typeparam name="TKey">See IRepository.</typeparam>
    public abstract class RepositoryBase<TEntity, TKey> : IRepository<TEntity, TKey>
    {
        /// <summary>
        /// See IRepository.
        /// </summary>
        public IUnitOfWork UnitOfWork { get; set; }

        protected RepositoryBase() { }

        protected RepositoryBase(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// See IRepository.
        /// </summary>
        public TEntity Get(TKey key)
        {
            this.CheckUnitOfWorkAssigned();

            return this.UnitOfWork.Get<TEntity, TKey>(key);
        }

        /// <summary>
        /// See IRepository.
        /// </summary>
        public IEnumerable<TEntity> GetAll()
        {
            this.CheckUnitOfWorkAssigned();

            return this.UnitOfWork.Query<TEntity>().ToList();
        }

        /// <summary>
        /// See IRepository.
        /// </summary>
        public TResult GetFor<TResult>(Func<IQueryable<TEntity>, TResult> queryBuilder)
        {
            this.CheckUnitOfWorkAssigned();

            return this.UnitOfWork.Query(queryBuilder);
        }

        /// <summary>
        /// See IRepository.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> GetList(Func<IQueryable<TEntity>, IEnumerable<TEntity>> queryBuilder)
        {
            return this.GetFor(queryBuilder);
        }

        /// <summary>
        /// See IRepository.
        /// </summary>
        public void Save(TEntity entity)
        {
            this.CheckUnitOfWorkAssigned();

            this.UnitOfWork.Save(entity);
        }

        /// <summary>
        /// See IRepository.
        /// </summary>
        public void Replace(IEnumerable<TEntity> entityList, Func<TEntity, TEntity, bool> equalityComparer)
        {
            this.CheckUnitOfWorkAssigned();

            var existingEntityList = this.GetAll();
            var listToDelete = existingEntityList.Where(entity => !entityList.Any(x => equalityComparer(x, entity))).ToList();

            foreach (var entity in listToDelete)
                this.UnitOfWork.Delete(entity);

            foreach (var entity in entityList)
                this.UnitOfWork.Save(entity);
        }

        /// <summary>
        /// See IRepository.
        /// </summary>
        public void Delete(TEntity entity)
        {
            this.CheckUnitOfWorkAssigned();

            this.UnitOfWork.Delete(entity);
        }

        /// <summary>
        /// Checks to see if a IUnitOfWork instance is set and if it is valid. If not, throws.
        /// </summary>
        protected void CheckUnitOfWorkAssigned()
        {
            if (this.UnitOfWork != null && !this.UnitOfWork.IsValid)
                this.UnitOfWork = null;

            if (this.UnitOfWork == null) 
                throw new DataAccessException(DataAccessExceptionReason.UnitOfWorkNotAssigned);
        }
    }
}