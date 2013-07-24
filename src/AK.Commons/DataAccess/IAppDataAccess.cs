/*******************************************************************************************************************************
 * AK.Commons.DataAccess.IAppDataAccess
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

namespace AK.Commons.DataAccess
{
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
}