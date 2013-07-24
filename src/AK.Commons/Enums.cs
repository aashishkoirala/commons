/*******************************************************************************************************************************
 * AK.Commons.Enums
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

namespace AK.Commons
{
    /// <summary>
    /// Reason codes for InitializationException.
    /// </summary>
    public enum InitializationExceptionReason
    {
        CouldNotInitialize,
        CouldNotInitializeComposer,
        CouldNotInitializeConfig,
        CouldNotInitializeLogger,
        AlreadyInitialized,
        ApplicationNotInitialized,
        ConfigNotEnabled,
        LoggingNotEnabled,
        InvalidInitializationOptions
    }
    
    namespace Logging
    {
        /// <summary>
        /// Logging levels.
        /// </summary>
        public enum LogLevel
        {
            Verbose,
            Information,
            Warning,
            Error
        }
    }

    namespace Composition
    {
        /// <summary>
        /// Reason codes for ComposerException.
        /// </summary>
        public enum ComposerExceptionReason
        {
            NoExports,
            TooManyExports            
        }
    }

    namespace Configuration
    {
        /// <summary>
        /// Reason codes for AppConfigException.
        /// </summary>
        public enum AppConfigExceptionReason
        {
            CouldNotRetrieveConfiguration,
            CouldNotInitializeConfiguration,
            ConfigKeyNotFound,
            ConfigKeyOfWrongType,
            ConfigStoreError
        }
    }

    namespace DataAccess
    {
        /// <summary>
        /// Reason codes for DataAccessException.
        /// </summary>
        public enum DataAccessExceptionReason
        {
            InitializationFailed,
            OperationFailed,
            UnitOfWorkNotAssigned
        }
    }

    namespace Exceptions
    {
        /// <summary>
        /// The one and only reason for GeneralException.
        /// </summary>
        public enum GeneralExceptionReason
        {
            General
        }
    }

    namespace Web.Security
    {
        /// <summary>
        /// Web authentication result types.
        /// </summary>
        public enum WebAuthenticationResultType
        {
            Success,
            Denied,
            Error
        }
    }
}