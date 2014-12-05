/*******************************************************************************************************************************
 * AK.Commons.Aspects.CodeGeneratedEventArgs
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
using System.CodeDom;

#endregion

namespace AK.Commons.Aspects
{
    /// <summary>
    /// Arguments for the AspectHelper.CodeGenerated event.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class CodeGeneratedEventArgs : EventArgs
    {
        public CodeGeneratedEventArgs(
            Type contractType,
            Type implementationType,
            string generatedCode,
            CodeCompileUnit compileUnit)
        {
            this.ContractType = contractType;
            this.ImplementationType = implementationType;
            this.GeneratedCode = generatedCode;
            this.CompileUnit = compileUnit;
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
        /// The generated code in C#. For inspection only.
        /// </summary>
        public string GeneratedCode { get; private set; }

        /// <summary>
        /// The CodeCompileUnit that will be used for compilation. You
        /// can make changes to customize the behavior of the generated type.
        /// </summary>
        public CodeCompileUnit CompileUnit { get; private set; }
    }
}