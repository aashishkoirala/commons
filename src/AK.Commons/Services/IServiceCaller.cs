/*******************************************************************************************************************************
 * AK.Commons.Services.IServiceCaller
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

using System;

namespace AK.Commons.Services
{
    /// <summary>
    /// Provides a way to call WCF service operations and handles opening and closing of the channel.
    /// </summary>
    /// <typeparam name="TChannel">WCF channel contract type.</typeparam>
    /// <author>Aashish Koirala</author>
    public interface IServiceCaller<out TChannel>
    {
        /// <summary>
        /// Calls the given WCF service operation.
        /// </summary>
        /// <param name="action">Operation to call that does not return a value.</param>
        void Call(Action<TChannel> action);

        /// <summary>
        /// Calls the given WCF service operation.
        /// </summary>
        /// <typeparam name="TResult">Return type of the operation.</typeparam>
        /// <param name="action">Operation to call that does not return a value.</param>
        /// <returns>Return value from the operation.</returns>
        TResult Call<TResult>(Func<TChannel, TResult> action);
    }
}