/*******************************************************************************************************************************
 * AK.Commons.Providers.DataAccess.AppDataAccess
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
using AK.Commons.Composition;
using AK.Commons.Configuration;
using AK.Commons.DataAccess;
using AK.Commons.Exceptions;
using AK.Commons.Logging;

#endregion

namespace AK.Commons.Providers.DataAccess
{
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

            factory = this.CreateFactory(name);
            factories[name] = factory;

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
            catch(Exception ex)
            {
                ex.WrapLogAndThrow<DataAccessException, DataAccessExceptionReason>(
                    DataAccessExceptionReason.InitializationFailed);
            }

            return factory;
        }

        #endregion
    }
}