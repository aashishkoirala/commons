/*******************************************************************************************************************************
 * AK.Commons.DomainDriven.DomainDrivenUtility
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

#region Namespace Imports

using AK.Commons.Configuration;
using AK.Commons.DataAccess;
using System;
using System.Collections.Generic;

#endregion

namespace AK.Commons.DomainDriven
{
    /// <summary>
    /// Utility methods related to DDD.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class DomainDrivenUtility
    {
        #region Properties

        /// <summary>
        /// Override this method to supply IDomainRepository instances for testing. Do NOT
        /// override this method in production code.
        /// </summary>
        public static Func<Type, IDomainRepository> DomainRepositoryFactory { get; set; }

        #endregion

        #region IUnitOfWorkFactory Extensions

        /// <summary>
        /// Adds the given domain repository as a participant in the unit of work that will be created
        /// from this factory.
        /// </summary>
        /// <param name="factory">Unit of work factory.</param>
        /// <param name="repository">Domain repository.</param>
        /// <returns>Unit of work factory with the domain repository added as a participant.</returns>
        public static IUnitOfWorkFactory With(this IUnitOfWorkFactory factory, IDomainRepository repository)
        {
            return new UnitOfWorkFactory(factory, repository.GetType(), repository);
        }

        /// <summary>
        /// Adds a domain repository of the given type as a participant in the unit of work that will be created
        /// from this factory.
        /// </summary>
        /// <typeparam name="T">Type of domain repository.</typeparam>
        /// <param name="factory">Unit of work factory.</param>
        /// <returns>Unit of work factory with the domain repository added as a participant.</returns>
        public static IUnitOfWorkFactoryWithRepositoryMap With<T>(this IUnitOfWorkFactory factory)
            where T : IDomainRepository
        {
            var repository = DomainRepositoryFactory != null
                                 ? DomainRepositoryFactory(typeof (T))
                                 : AppEnvironment.Composer.Resolve<T>();

            return new UnitOfWorkFactory(factory, typeof (T), repository);
        }

        /// <summary>
        /// Executes the given action within a unit of work created using the given factory.
        /// </summary>
        /// <param name="factory">Unit-of-work factory.</param>
        /// <param name="action">Action to execute.</param>
        public static void Execute(this IUnitOfWorkFactory factory, Action action)
        {
            using (var unit = factory.Create())
            {
                action();
                unit.Commit();
            }
        }

        /// <summary>
        /// Executes the given action within a unit of work created using the given factory.
        /// </summary>
        /// <typeparam name="T">Return type of action.</typeparam>
        /// <param name="factory">Unit-of-work factory.</param>
        /// <param name="action">Action to execute.</param>
        /// <returns>Result of action.</returns>
        public static T Execute<T>(this IUnitOfWorkFactory factory, Func<T> action)
        {
            using (var unit = factory.Create())
            {
                var result = action();
                unit.Commit();
                return result;
            }
        }

        /// <summary>
        /// Executes the given action within a unit of work created using the given factory.
        /// </summary>
        /// <param name="factory">Unit-of-work factory.</param>
        /// <param name="action">Action to execute.</param>
        public static void Execute(this IUnitOfWorkFactoryWithRepositoryMap factory, Action<IDomainRepositoryMap> action)
        {
            using (var unit = factory.Create())
            {
                action(factory.Map);
                unit.Commit();
            }
        }

        /// <summary>
        /// Executes the given action within a unit of work created using the given factory.
        /// </summary>
        /// <typeparam name="T">Return type of action.</typeparam>
        /// <param name="factory">Unit-of-work factory.</param>
        /// <param name="action">Action to execute.</param>
        /// <returns>Result of action.</returns>
        public static T Execute<T>(this IUnitOfWorkFactoryWithRepositoryMap factory, Func<IDomainRepositoryMap, T> action)
        {
            using (var unit = factory.Create())
            {
                var result = action(factory.Map);
                unit.Commit();
                return result;
            }
        }

        #endregion

        #region Helper Types

        /// <summary>
        /// Special case of IUnitOfWorkFactory that is capable of holding
        /// a IDomainRepositoryMap instance.
        /// </summary>
        public interface IUnitOfWorkFactoryWithRepositoryMap : IUnitOfWorkFactory
        {
            /// <summary>
            /// The domain repository map.
            /// </summary>
            IDomainRepositoryMap Map { get; }
        }

        private class UnitOfWorkFactory : IUnitOfWorkFactoryWithRepositoryMap
        {
            private readonly IUnitOfWorkFactory factory;
            private readonly IDictionary<Type, IDomainRepository> repositories;

            public UnitOfWorkFactory(IUnitOfWorkFactory factory, Type contractType, IDomainRepository repository)
            {
                this.factory = factory;
                if (this.factory is UnitOfWorkFactory) this.repositories = ((UnitOfWorkFactory) factory).repositories;
                else this.repositories = new Dictionary<Type, IDomainRepository>();
                this.repositories[contractType] = repository;
            }

            public IDomainRepositoryMap Map { get { return new RepositoryMap(this.repositories); } }

            public void Configure(IAppConfig config, string name)
            {
                this.factory.Configure(config, name);
            }

            public IUnitOfWork Create()
            {
                var unit = this.factory.Create();
                foreach (var repository in this.repositories.Values)
                    repository.AssignUnitOfWork(unit);
                return unit;
            }
        }

        private class RepositoryMap : IDomainRepositoryMap
        {
            private readonly IDictionary<Type, IDomainRepository> map;

            public RepositoryMap(IDictionary<Type, IDomainRepository> map)
            {
                this.map = map;
            }

            public T Get<T>() where T : IDomainRepository
            {
                return (T)this.map[typeof (T)];
            }
        }

        #endregion
    }
}