/*******************************************************************************************************************************
 * AK.Commons.DataAccess.UnitOfWorkUtility
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
using AK.Commons.Exceptions;

#endregion

namespace AK.Commons.DataAccess
{
    /// <summary>
    /// Convenience utility/extension methods around unit-of-work.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class UnitOfWorkUtility
    {
        /// <summary>
        /// Executes the given action within a new unit of work; handles commits and rollbacks
        /// and exception wrapping.
        /// </summary>
        /// <param name="unitOfWorkFactory">Unit-of-work factory to use.</param>
        /// <param name="action">Action to execute.</param>
        public static void Execute(
            this IUnitOfWorkFactory unitOfWorkFactory, Action<IUnitOfWork> action)
        {
            using (var unitOfWork = unitOfWorkFactory.Create())
            {
                try
                {
                    action(unitOfWork);
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    ex.WrapLogAndThrow<DataAccessException, DataAccessExceptionReason>(DataAccessExceptionReason.OperationFailed);
                }
            }
        }

        /// <summary>
        /// Executes the given action within a new unit of work; handles commits and rollbacks
        /// and exception wrapping.
        /// </summary>
        /// <param name="unitOfWorkFactoryWithRepositoryAssignment">Unit-of-work factory to use.</param>
        /// <param name="action">Action to execute.</param>
        public static void Execute(this UnitOfWorkFactoryWithRepositoryAssignment 
            unitOfWorkFactoryWithRepositoryAssignment, Action<IUnitOfWork> action)
        {
            unitOfWorkFactoryWithRepositoryAssignment.UnitOfWorkFactory.Execute(unitOfWork =>
            {
                unitOfWork.AssignTo(unitOfWorkFactoryWithRepositoryAssignment.Repositories);
                action(unitOfWork);
            });
        }

        /// <summary>
        /// Returns an object that you can call Execute on and has all given repositories
        /// assigned with the appropriate IUnitOfWork instances. Use this to follow this
        /// pattern:
        /// 
        ///   unitOfWorkFactory
        ///     .With(rep1, rep2, rep3, ...)
        ///     .Execute(unitOfWork => { ... });
        /// </summary>
        /// <param name="unitOfWorkFactory">Unit-of-work factory.</param>
        /// <param name="repositories">Repositories to involve in the Execute operation to follow.</param>
        /// <returns></returns>
        public static UnitOfWorkFactoryWithRepositoryAssignment With(
            this IUnitOfWorkFactory unitOfWorkFactory, params IRepository[] repositories)
        {
            return new UnitOfWorkFactoryWithRepositoryAssignment
            {
                UnitOfWorkFactory = unitOfWorkFactory,
                Repositories = repositories
            };
        }

        /// <summary>
        /// Assigns the given IUnitOfWork instance to all provided repositories.
        /// </summary>
        /// <param name="unitOfWork">IUnitOfWork instance.</param>
        /// <param name="repositories">List of repositories.</param>
        public static void AssignTo(this IUnitOfWork unitOfWork, IEnumerable<IRepository> repositories)
        {
            repositories.ForEach(x => x.UnitOfWork = unitOfWork);
        }

        /// <summary>
        /// Helper data structure for use with the "With/Execute" pattern.
        /// </summary>
        public class UnitOfWorkFactoryWithRepositoryAssignment
        {
            public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }
            public IEnumerable<IRepository> Repositories { get; set; } 
        }
    }
}