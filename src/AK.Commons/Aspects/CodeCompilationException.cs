/*******************************************************************************************************************************
 * AK.Commons.Aspects.CodeCompilationException
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

using System;
using System.CodeDom.Compiler;

#endregion

namespace AK.Commons.Aspects
{
    /// <summary>
    /// Exception thrown when there is a problem compiling aspect-wrapped generated code.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class CodeCompilationException : ApplicationException
    {
        private const string DefaultMessage = "An error occurred while trying to compile the aspectized code.";

        /// <summary>
        /// Results of compilation operation.
        /// </summary>
        public CompilerResults Results { get; private set; }

        /// <summary>
        /// Creates a new instance of CodeCompilationException.
        /// </summary>
        /// <param name="results">Results of compilation operation.</param>
        public CodeCompilationException(CompilerResults results) : base(DefaultMessage)
        {
            this.Results = results;
        }
    }
}