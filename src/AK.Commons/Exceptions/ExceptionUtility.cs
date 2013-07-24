/*******************************************************************************************************************************
 * AK.Commons.Exceptions.ExceptionUtility
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
using AK.Commons.Logging;

#endregion

namespace AK.Commons.Exceptions
{
    /// <summary>
    /// Convenience utility/extension methods for exceptions.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class ExceptionUtility
    {
        /// <summary>
        /// Wraps the given exception as a reasoned exception of the given type.
        /// </summary>
        /// <typeparam name="TException">Reasoned exception type.</typeparam>
        /// <typeparam name="TReasonEnum">Reasoned exception reason code enum type.</typeparam>
        /// <param name="exception">Excepion to wrap.</param>
        /// <param name="reason">Reason code.</param>
        /// <param name="message">Additional message, if any.</param>
        /// <returns>Reasoned exception instance.</returns>
        public static TException Wrap<TException, TReasonEnum>(this Exception exception, 
            TReasonEnum reason, string message = null)
            where TException : ReasonedException<TReasonEnum> 
            where TReasonEnum : struct 
        {
            var args = message == null ? new object[] {reason, exception} : new object[] {reason, message, exception};
            return (TException) Activator.CreateInstance(typeof (TException), args);
        }

        /// <summary>
        /// Wraps the given exception as a reasoned exception of the given type. Also logs
        /// the exception.
        /// </summary>
        /// <typeparam name="TException">Reasoned exception type.</typeparam>
        /// <typeparam name="TReasonEnum">Reasoned exception reason code enum type.</typeparam>
        /// <param name="exception">Excepion to wrap.</param>
        /// <param name="reason">Reason code.</param>
        /// <param name="logger">Logger to use (uses AppEnvironment.Logger by default if nothing is passed in).</param>
        /// <param name="message">Additional message, if any.</param>
        /// <returns>Reasoned exception instance.</returns>
        public static TException WrapAndLog<TException, TReasonEnum>(this Exception exception,
            TReasonEnum reason, IAppLogger logger = null, string message = null)
            where TException : ReasonedException<TReasonEnum>
            where TReasonEnum : struct
        {
            if(logger == null) logger = AppEnvironment.Logger;

            var wrappedException = exception.Wrap<TException, TReasonEnum>(reason, message);

            logger.Error(wrappedException);

            return wrappedException;
        }

        /// <summary>
        /// Wraps the given exception as a reasoned exception of the given type and throws the
        /// reasoned exception.
        /// </summary>
        /// <typeparam name="TException">Reasoned exception type.</typeparam>
        /// <typeparam name="TReasonEnum">Reasoned exception reason code enum type.</typeparam>
        /// <param name="exception">Excepion to wrap.</param>
        /// <param name="reason">Reason code.</param>
        /// <param name="message">Additional message, if any.</param>
        public static void WrapAndThrow<TException, TReasonEnum>(this Exception exception,
            TReasonEnum reason, string message = null)
            where TException : ReasonedException<TReasonEnum>
            where TReasonEnum : struct
        {
            var args = message == null ? new object[] { reason, exception } : new object[] { reason, message, exception };
            var exceptionToThrow = (TException)Activator.CreateInstance(typeof(TException), args);

            throw exceptionToThrow;
        }

        /// <summary>
        /// Wraps the given exception as a reasoned exception of the given type. Also logs
        /// the exception. Then throws the reasoned exception.
        /// </summary>
        /// <typeparam name="TException">Reasoned exception type.</typeparam>
        /// <typeparam name="TReasonEnum">Reasoned exception reason code enum type.</typeparam>
        /// <param name="exception">Excepion to wrap.</param>
        /// <param name="reason">Reason code.</param>
        /// <param name="logger">Logger to use (uses AppEnvironment.Logger by default if nothing is passed in).</param>
        /// <param name="message">Additional message, if any.</param>
        public static void WrapLogAndThrow<TException, TReasonEnum>(this Exception exception,
            TReasonEnum reason, IAppLogger logger = null, string message = null)
            where TException : ReasonedException<TReasonEnum>
            where TReasonEnum : struct
        {
            var exceptionToThrow = exception.WrapAndLog<TException, TReasonEnum>(reason, logger, message);

            throw exceptionToThrow;
        }
    }
}