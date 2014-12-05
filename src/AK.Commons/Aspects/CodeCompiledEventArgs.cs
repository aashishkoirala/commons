/*******************************************************************************************************************************
 * AK.Commons.Aspects.CodeCompiledEventArgs
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
    /// Arguments for the AspectHelper.CodeCompiled event.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class CodeCompiledEventArgs : EventArgs
    {
        public CodeCompiledEventArgs(
            Type contractType,
            Type implementationType,
            CompilerResults results)
        {
            this.ContractType = contractType;
            this.ImplementationType = implementationType;
            this.Results = results;
        }

        /// <summary>
        /// Contract type.
        /// </summary>
        public Type ContractType { get; private set; }

        /// <summary>
        /// Implementation type.
        /// </summary>
        public Type ImplementationType { get; private set; }

        /// <summary>
        /// Compiler results.
        /// </summary>
        public CompilerResults Results { get; private set; }
    }
}