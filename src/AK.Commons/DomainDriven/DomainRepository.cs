/*******************************************************************************************************************************
 * AK.Commons.DomainDriven.DomainRepository
 * Copyright © 2014 Aashish Koirala <http://aashishkoirala.github.io>
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

using AK.Commons.DataAccess;

namespace AK.Commons.DomainDriven
{
    /// <summary>
    /// Represents a domain repository that can be scoped to a unit-of-work.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IDomainRepository
    {
        /// <summary>
        /// Scopes the repository to the given unit-of-work.
        /// </summary>
        /// <param name="unitOfWork">Unit of work.</param>
        void AssignUnitOfWork(IUnitOfWork unitOfWork);
    }

    /// <summary>
    /// Represents something that can give you a given type of domain repository.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IDomainRepositoryMap
    {
        /// <summary>
        /// Gets the domain repository of given type.
        /// </summary>
        /// <typeparam name="T">Type of domain repository.</typeparam>
        /// <returns>Domain repository.</returns>
        T Get<T>() where T : IDomainRepository;
    }

    /// <summary>
    /// Default implementation of IDomainRepository, it is recommended that
    /// all domain repositories inherit from this class if possible.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public abstract class DomainRepositoryBase : IDomainRepository
    {
        /// <summary>
        /// Unit of work that this domain repository is scoped to.
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; private set; }

        /// <summary>
        /// Scopes the repository to the given unit-of-work.
        /// </summary>
        /// <param name="unitOfWork">Unit of work.</param>
        public void AssignUnitOfWork(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }
    }
}