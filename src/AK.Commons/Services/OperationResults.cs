/*******************************************************************************************************************************
 * AK.Commons.Services.OperationResults
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

#endregion

namespace AK.Commons.Services
{
    /// <summary>
    /// Represents a sequence of results from a service operation.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [DataContract]
    public class OperationResults
    {
        /// <summary>
        /// Underlying list of OperationResult instances.
        /// </summary>
        [DataMember]
        public ICollection<OperationResult> Results { get; private set; }

        /// <summary>
        /// Creates an empty result sequence.
        /// </summary>
        public OperationResults()
        {
            this.Results = new Collection<OperationResult>();
        }

        /// <summary>
        /// Creates an instance with the given result sequence.
        /// </summary>
        /// <param name="results">Result sequence.</param>
        public OperationResults(IEnumerable<OperationResult> results)
        {
            this.Results = new Collection<OperationResult>(results.ToList());
        }

        /// <summary>
        /// Is TRUE if all contained results are successful, FALSE otherwise.
        /// </summary>
        public bool IsSuccess
        {
            get { return this.Results.All(x => x.IsSuccess); }
        }

        /// <summary>
        /// Implicitly casts OperationResults to OperationResult array using
        /// the Results property.
        /// </summary>
        /// <param name="results">OperationResults instance.</param>
        /// <returns>Array of OperationResult objects.</returns>
        public static implicit operator OperationResult[](OperationResults results)
        {
            return results.Results.ToArray();
        }
    }

    /// <summary>
    /// Represents a sequence of results from a service operation.
    /// </summary>
    /// <typeparam name="TResult">Type of each result.</typeparam>
    /// <author>Aashish Koirala</author>
    [DataContract]
    public class OperationResults<TResult>
    {
        /// <summary>
        /// Underlying list of OperationResult instances.
        /// </summary>
        [DataMember]
        public ICollection<OperationResult<TResult>> Results { get; private set; }

        /// <summary>
        /// Creates an empty result sequence.
        /// </summary>
        public OperationResults()
        {
            this.Results = new Collection<OperationResult<TResult>>();
        }

        /// <summary>
        /// Creates an instance with the given result sequence.
        /// </summary>
        /// <param name="results">Result sequence.</param>
        public OperationResults(IEnumerable<OperationResult<TResult>> results)
        {
            this.Results = new Collection<OperationResult<TResult>>(results.ToList());
        }

        /// <summary>
        /// Is TRUE if all contained results are successful, FALSE otherwise.
        /// </summary>
        public bool IsSuccess
        {
            get { return this.Results.All(x => x.IsSuccess); }
        }

        /// <summary>
        /// Implicitly casts OperationResults to OperationResult array using
        /// the Results property.
        /// </summary>
        /// <typeparam name="TResult">Type of each result.</typeparam>
        /// <param name="results">OperationResults instance.</param>
        /// <returns>Array of OperationResult objects.</returns>
        public static implicit operator OperationResult<TResult>[](OperationResults<TResult> results)
        {
            return results.Results.ToArray();
        }
    }
}