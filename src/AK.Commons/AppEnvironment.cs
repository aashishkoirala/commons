/*******************************************************************************************************************************
 * AK.Commons.AppEnvironment
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
using System.IO;
using System.Reflection;
using AK.Commons.Composition;
using AK.Commons.Configuration;
using AK.Commons.DataAccess;
using AK.Commons.Exceptions;
using AK.Commons.Logging;
using AK.Commons.Providers.Composition;
using AK.Commons.Providers.Configuration;
using AK.Commons.Providers.DataAccess;
using AK.Commons.Providers.Logging;
using AK.Commons.Services.Client;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using AK.Commons.Web.Bundling;

#endregion

namespace AK.Commons
{
    /// <summary>
    /// Initializes the application environment and provides common services such as logging,
    /// composition and configuration to the application.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class AppEnvironment
    {
        #region Fields

        /// <summary>
        /// The one and only instance of the composer.
        /// </summary>
        private static IComposer composer;

        /// <summary>
        /// The one and only instance of the application configuration store.
        /// </summary>
        private static IAppConfig config;

        /// <summary>
        /// The one and only instance of the application level logger.
        /// </summary>
        private static IAppLogger logger;

        /// <summary>
        /// The one and only instance of the application level data access interface.
        /// </summary>
        private static IAppDataAccess dataAccess;

        #endregion

        #region Properties

        /// <summary>
        /// Whether the application environment is initialized.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// The application level composer that provides access to composition services and
        /// wraps MEF stuff. If possible, use an "Import"ed instance of IComposer rather
        /// than accessing this directly.
        /// </summary>
        /// <exception cref="InitializationException">
        /// If you access this before calling Initialize.
        /// </exception>
        [Export(typeof (IComposer))]
        public static IComposer Composer
        {
            get
            {
                if (!IsInitialized || composer == null)
                    throw new InitializationException(InitializationExceptionReason.ApplicationNotInitialized);

                return composer;
            }
        }

        /// <summary>
        /// The application level config interface that provides access to configuration settings.
        /// If possible, use an "Import"ed instance of IAppConfig rather than accessing this directly.
        /// </summary>
        /// <exception cref="InitializationException">
        /// If you access this before calling Initialize, or if you try to access this but you
        /// did not enable configuration by providing a proper IConfigStore instance during
        /// initialization.
        /// </exception>
        [Export(typeof (IAppConfig))]
        public static IAppConfig Config
        {
            get
            {
                if (!IsInitialized)
                    throw new InitializationException(InitializationExceptionReason.ApplicationNotInitialized);

                if (config == null)
                    throw new InitializationException(InitializationExceptionReason.ConfigNotEnabled);

                return config;
            }
        }

        /// <summary>
        /// The application level logging interface that handles all logging.
        /// If possible, use an "Import"ed instance of IAppLogger rather than accessing this directly.
        /// </summary>
        /// <exception cref="InitializationException">
        /// If you access this before calling Initialize, or if you try to access this but you
        /// did not enable logging during initialization.
        /// </exception>
        [Export(typeof (IAppLogger))]
        public static IAppLogger Logger
        {
            get
            {
                if (!IsInitialized)
                    throw new InitializationException(InitializationExceptionReason.ApplicationNotInitialized);

                if (logger == null)
                    throw new InitializationException(InitializationExceptionReason.LoggingNotEnabled);

                return logger;
            }
        }

        /// <summary>
        /// The application level data access interface that handles all data access.
        /// If possible, use an "Import"ed instance of IAppDataAccess rather than accessing this directly.
        /// </summary>
        /// <exception cref="InitializationException">
        /// If you access this before calling Initialize.
        /// </exception>
        [Export(typeof (IAppDataAccess))]
        public static IAppDataAccess DataAccess
        {
            get
            {
                if (!IsInitialized)
                    throw new InitializationException(InitializationExceptionReason.ApplicationNotInitialized);

                return dataAccess;
            }
        }

        /// <summary>
        /// The application level web-based bundling configurator. If possible, use an "Import"ed instance of
        /// IBundleConfigurator rather than accessing this directly.
        /// </summary>
        /// <exception cref="InitializationException">
        /// If you access this before calling Initialize.
        /// </exception>
        [Export(typeof (IBundleConfigurator))]
        public static IBundleConfigurator BundleConfigurator
        {
            get
            {
                if (!IsInitialized)
                    throw new InitializationException(InitializationExceptionReason.ApplicationNotInitialized);

                return BundleConfiguratorFactory.Create(composer, config);
            }
        }

        #endregion

        #region Methods (Public)

        /// <summary>
        /// Initializes the application environment. This method must be called on application startup
        /// before anything else and after you have a valid IConfigStore instance, if you're going to
        /// enable configuration. This method performs the following key actions, among others:
        /// 1) Initializes the composer- which handles MEF composition.
        /// 2) Initializes the config interface, if a valid IConfigStore instance has been provided.
        /// 3) Initializes the application logger, if specified.
        /// 4) Initializes the data access interface.
        /// 5) Autogenerates clients for WCF services, if specified.
        /// </summary>
        /// <param name="applicationName">
        /// Unique name for the application. Used, among other things, to:
        /// 1) Retrieve application-specific settings from the configuration store.
        /// 2) Include in log entries.
        /// </param>
        /// <param name="initializationOptions">
        /// Initialization options. Skip to include default values.
        /// See <see cref="InitializationOptions"/> for details.
        /// </param>
        /// <exception cref="InitializationException">
        /// Thrown if initialization fails in any step, or if the application environment has
        /// already been initialized.
        /// </exception>
        public static void Initialize(string applicationName, InitializationOptions initializationOptions = null)
        {
            if (IsInitialized)
                throw new InitializationException(InitializationExceptionReason.AlreadyInitialized);

            // This logger is used for rudimentary logging during initialization until the logging
            // system is initialized. See "InitializationLogger" class declaration for more details.
            //
            var initializationLogger = new InitializationLogger();

            if (initializationOptions == null)
            {
                initializationOptions = new InitializationOptions
                {
                    EnableLogging = false, 
                    GenerateServiceClients = false
                };
            }

            ValidateInitializationOptions(initializationOptions);

            // Initialization is atomic, or at least we would like it to be.
            //
            try
            {
                AppConfig configImpl = null;
                if (initializationOptions.ConfigStore != null)
                {
                    configImpl = new AppConfig(applicationName, initializationOptions.ConfigStore, initializationLogger,
                                               initializationOptions.ConfigTempLocation);
                }

                var composerImpl = new Composer(configImpl, initializationLogger);

                AppLogger loggerImpl = null;
                if (initializationOptions.EnableLogging)
                    loggerImpl = new AppLogger(applicationName, composerImpl, initializationLogger);

                var dataAccessImpl = new AppDataAccess(composerImpl, configImpl, loggerImpl);

                if (initializationOptions.GenerateServiceClients)
                    GenerateServiceClients(composerImpl);

                // TODO: Other initialization stuff goes here.

                composer = composerImpl;
                config = configImpl;
                logger = loggerImpl;
                dataAccess = dataAccessImpl;

                IsInitialized = true;
            }
            catch (InitializationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                exception.WrapAndThrow<InitializationException, InitializationExceptionReason>(
                    InitializationExceptionReason.CouldNotInitialize);
            }
        }

        public static void ShutDown()
        {
            if (!IsInitialized)
                throw new InitializationException(InitializationExceptionReason.ApplicationNotInitialized);

            // TODO: Other shut-down stuff goes here or somewhere around here.

            var appLogger = logger as AppLogger;
            if (appLogger == null) return;

            appLogger.ShutDown();

            IsInitialized = false;
        }

        #endregion

        #region Methods (Private)

        private static void GenerateServiceClients(IComposer composerImpl)
        {
            var serviceClientAssembly = ServiceClientUtility.GenerateServiceClients();
            var aggregateCatalog = composerImpl.Container.Catalog as AggregateCatalog;

            Debug.Assert(aggregateCatalog != null);

            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(serviceClientAssembly));
        }

        private static void ValidateInitializationOptions(InitializationOptions initializationOptions)
        {
            if (initializationOptions.ConfigStore == null &&
                (initializationOptions.EnableLogging || initializationOptions.GenerateServiceClients))
                throw new InitializationException(InitializationExceptionReason.InvalidInitializationOptions);
        }

        #endregion

        #region Nested Type (InitializationLogger)

        /// <summary>
        /// This is a rudimentary implementation of IAppLogger that provides basic logging
        /// until the main logging system is initialized. It just writes to a local file
        /// alongside the entry assembly.
        /// </summary>
        private class InitializationLogger : IAppLogger
        {
            private readonly string logFile;
            private static readonly object LogFileLock = new object();

            public InitializationLogger()
            {
                this.logFile = string.Format("{0}.log", 
                    (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).Location);
                File.WriteAllText(this.logFile, string.Empty);
            }

            public void Log(LogLevel logLevel, string message)
            {
                var entry = string.Format("{0} | {1} | {2}{3}",
                    DateTime.Now, logLevel, message, Environment.NewLine);

                lock (LogFileLock)
                {
                    File.AppendAllText(this.logFile, entry);
                }
            }

            public void Verbose(string message)
            {
                this.Log(LogLevel.Verbose, message);
            }

            public void Information(string message)
            {
                this.Log(LogLevel.Information, message);
            }

            public void Warning(string message)
            {
                this.Log(LogLevel.Warning, message);
            }

            public void Error(string message)
            {
                this.Log(LogLevel.Error, message);
            }

            public void Warning(Exception exception)
            {
                this.Log(LogLevel.Warning, exception.ToString());
            }

            public void Error(Exception exception)
            {
                this.Log(LogLevel.Error, exception.ToString());
            }
        }

        #endregion        
    }
}