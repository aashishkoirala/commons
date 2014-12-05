/*******************************************************************************************************************************
 * AK.Commons.DataAccess.AppDataAccess
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

#region Namespace Imports

using AK.Commons.Composition;
using AK.Commons.Configuration;
using AK.Commons.Exceptions;
using AK.Commons.Logging;
using System;
using System.Collections.Generic;

#endregion

namespace AK.Commons.DataAccess
{
    #region IAppDataAccess

    /// <summary>
    /// Interface that provides data access facilities to the application. You should not directly implement
    /// this interface or register another MEF export for this. The application environment expects only
    /// one implementation for this which it provides. To implement your own data access layer, you would create
    /// a new unit-of-work factory by implementing <see cref="IUnitOfWorkFactory"/> and register it using
    /// configuration.
    /// 
    /// In order to use the system implementation of this, you need to:
    /// 1) Make sure you've called AppEnvironment.Initialize().
    /// 2) Either:
    ///    a) Create a MEF import against IAppDataAccess (preferably), or:
    ///    b) Use AppEnvironment.DataAccess.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IAppDataAccess
    {
        /// <summary>
        /// Gets the unique-of-work factory with the given name.
        /// </summary>
        /// <param name="name">Factory name.</param>
        IUnitOfWorkFactory this[string name] { get; }

        /// <summary>
        /// Gets the default unit-of-work factory (i.e. one without a name).
        /// </summary>
        IUnitOfWorkFactory Default { get; }
    }

    #endregion

    #region AppDataAccess

    /// <summary>
    /// The one and only internal implementation of IAppDataAccess.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class AppDataAccess : IAppDataAccess
    {
        #region Constants and Fields

        private const string ConfigNameKeyFormat = "ak.commons.dataaccess.uowfactory.{0}";
        private const string ConfigProviderNameKeyFormat = "ak.commons.dataaccess.uowfactory.{0}.provider";

        private readonly IComposer composer;
        private readonly IAppConfig config;
        private readonly IAppLogger logger;

        private readonly IDictionary<string, IUnitOfWorkFactory> factories =
            new Dictionary<string, IUnitOfWorkFactory>();
        private readonly static object factoriesLock = new object();

        #endregion

        #region Constructor

        public AppDataAccess(IComposer composer, IAppConfig config, IAppLogger logger)
        {
            this.composer = composer;
            this.config = config;
            this.logger = logger;
        }

        #endregion

        #region Properties (IAppDataAccess)

        public IUnitOfWorkFactory this[string name]
        {
            get { return this.GetFactory(name); }
        }

        public IUnitOfWorkFactory Default
        {
            get { return this.GetFactory(null); }
        }

        #endregion

        #region Methods (Private)

        private IUnitOfWorkFactory GetFactory(string name)
        {
            if (name == null) name = string.Empty;

            IUnitOfWorkFactory factory;
            if (this.factories.TryGetValue(name, out factory))
                return factory;

            lock (factoriesLock)
            {
                if (this.factories.TryGetValue(name, out factory))
                    return factory;

                factory = this.CreateFactory(name);
                factories[name] = factory;
            }

            return factory;
        }

        private IUnitOfWorkFactory CreateFactory(string name)
        {
            var nameParam = string.Format(ConfigNameKeyFormat, name);
            var providerKey = string.Format(ConfigProviderNameKeyFormat, name);

            this.logger.Verbose(string.Format("Creating IUnitOfWorkFactory with name {0}; name key: {1}, " +
                "provider key: {2}", name, nameParam, providerKey));

            var provider = this.config.Get<string>(providerKey);

            this.logger.Verbose(string.Format("Using provider \"{0}\"", provider));

            IUnitOfWorkFactory factory = null;

            try
            {
                factory = this.composer.Resolve<IUnitOfWorkFactory, IProviderMetadata>(
                    metadata => metadata.Provider == provider);

                factory.Configure(this.config, nameParam);

            }
            catch (Exception ex)
            {
                ex.WrapLogAndThrow<DataAccessException, DataAccessExceptionReason>(
                    DataAccessExceptionReason.InitializationFailed);
            }

            return factory;
        }

        #endregion
    }

    #endregion
}