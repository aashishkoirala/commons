/*******************************************************************************************************************************
 * AK.Commons.Aspects.Constructs
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
    /// Common constructs used throughout the library.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal static class Constructs
    {
        public static readonly CodeExpression ReturnValueExpression;
        public static readonly CodeExpression BoxedReturnValueExpression;
        public static readonly CodeStatement ReturnValueStatement;
        public static readonly CodeExpression TargetFieldExpression;
        public static readonly CodeExpression MethodStartExpression;
        public static readonly CodeExpression NowExpression;
        public static readonly CodeExpression ValueExpression;

        static Constructs()
        {
            ReturnValueExpression = new CodeVariableReferenceExpression(VariableNames.ReturnValue);
            BoxedReturnValueExpression = new CodeVariableReferenceExpression(VariableNames.BoxedReturnValue);
            TargetFieldExpression =
                new CodeFieldReferenceExpression {FieldName = VariableNames.Target};
            ReturnValueStatement =
                new CodeMethodReturnStatement(ReturnValueExpression);
            MethodStartExpression = new CodeVariableReferenceExpression(VariableNames.MethodStart);
            NowExpression = new CodePropertyReferenceExpression(
                new CodeTypeReferenceExpression(typeof (DateTime)), "Now");
            ValueExpression = new CodeVariableReferenceExpression("value");
        }

        public static CodeCatchClause CatchClause
        {
            get { return new CodeCatchClause(VariableNames.Exception, new CodeTypeReference(typeof (Exception))); }
        }
    }
}