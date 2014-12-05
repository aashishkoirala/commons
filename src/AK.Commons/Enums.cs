/*******************************************************************************************************************************
 * AK.Commons.Enums
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

using System.Runtime.Serialization;

namespace AK.Commons
{
    /// <summary>
    /// Reason codes for InitializationException.
    /// </summary>
    public enum InitializationExceptionReason
    {
        [EnumDescription("Could not initialize.")] CouldNotInitialize,
        [EnumDescription("Could not initialize Composer.")] CouldNotInitializeComposer,
        [EnumDescription("Could not initialize Configuration.")] CouldNotInitializeConfig,
        [EnumDescription("Could not initialize Logger.")] CouldNotInitializeLogger,
        [EnumDescription("Application is already initialized.")] AlreadyInitialized,
        [EnumDescription("Application has not been initialized.")] ApplicationNotInitialized,
        [EnumDescription("Configuration has not been enabled.")] ConfigNotEnabled,
        [EnumDescription("Logging has not been enabled.")] LoggingNotEnabled,
        [EnumDescription("Invalid initialization options provided.")] InvalidInitializationOptions
    }

    namespace Logging
    {
        /// <summary>
        /// Logging levels.
        /// </summary>
        [DataContract]
        public enum LogLevel
        {
            [EnumMember] Diagnostic,
            [EnumMember] Verbose,
            [EnumMember] Information,
            [EnumMember] Warning,
            [EnumMember] Error
        }
    }

    namespace Composition
    {
        /// <summary>
        /// Reason codes for ComposerException.
        /// </summary>
        public enum ComposerExceptionReason
        {
            [EnumDescription("No matching exports were found.")] NoExports,
            [EnumDescription("Too many matching exports were found.")] TooManyExports
        }
    }

    namespace Configuration
    {
        /// <summary>
        /// Reason codes for AppConfigException.
        /// </summary>
        public enum AppConfigExceptionReason
        {
            [EnumDescription("There was an error trying to retrieve configuration settings.")] CouldNotRetrieveConfiguration,
            [EnumDescription("Could not initialize the configuration system.")] CouldNotInitializeConfiguration,
            [EnumDescription("The configuration setting was not found.")] ConfigKeyNotFound,
            [EnumDescription("The configuration setting was of a different type than requested.")] ConfigKeyOfWrongType,
            [EnumDescription("There was an error communicating with the configuration store.")] ConfigStoreError
        }
    }

    namespace DataAccess
    {
        /// <summary>
        /// Reason codes for DataAccessException.
        /// </summary>
        public enum DataAccessExceptionReason
        {
            [EnumDescription("Initialization of the data access layer failed.")] InitializationFailed,
            [EnumDescription("A database access operation failed.")] OperationFailed,
            [EnumDescription("The repository does not have its UnitOfWork set to a valid IUnitOfWork instance.")] UnitOfWorkNotAssigned
        }
    }

    namespace Exceptions
    {
        /// <summary>
        /// The one and only reason for GeneralException.
        /// </summary>
        public enum GeneralExceptionReason
        {
            [EnumDescription("An error has occurred.")] General
        }
    }
}