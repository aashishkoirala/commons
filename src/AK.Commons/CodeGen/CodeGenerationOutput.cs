/*******************************************************************************************************************************
 * AK.Commons.CodeGen.CodeGenerationOutput
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

#region Namespace Imports

using System.Reflection;
using System.CodeDom.Compiler;

#endregion

namespace AK.Commons.CodeGen
{
    /// <summary>
    /// Contains output information from a code-generation operation.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class CodeGenerationOutput
    {
        /// <summary>
        /// Whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The compiled assembly from the generated code.
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        /// The actual generated code.
        /// </summary>
        public string SourceCode { get; set; }

        /// <summary>
        /// The underlying results from compiler services.
        /// </summary>
        public CompilerResults CompilerResults { get; set; }
    }
}