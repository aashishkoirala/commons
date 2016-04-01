/*******************************************************************************************************************************
 * AK.Commons.DataAccess.IUnitOfWork
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

using System;

namespace AK.Commons.DataAccess
{
    /// <summary>
    /// Represents a unit-of-work in the unit-of-work pattern.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Whether the unit-of-work is still valid. Should be true when the unit is created,
        /// and set to false after a commit/rollback or when the unit is disposed.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets a repository for the given type scoped within this unit of work.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <returns>Repository instance.</returns>
        IRepository<T> Repository<T>() where T : class;

        /// <summary>
        /// Gets the next value in the given sequence.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="sequenceContainerName">Name of sequence container.</param>
        /// <param name="sequenceName">Name of sequence.</param>
        /// <param name="incrementBy">How much to increment the value by.</param>
        /// <returns>Next value in sequence.</returns>
        T NextValueInSequence<T>(string sequenceContainerName, string sequenceName, T incrementBy);

        /// <summary>
        /// Commits the unit of work.
        /// </summary>
        void Commit();

        /// <summary>
        /// Resets changes.
        /// </summary>
        void ResetChanges();
    }
}