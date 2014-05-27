/*******************************************************************************************************************************
 * AK.Commons.Aspects.Generators.ExitAspectGenerator
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
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace AK.Commons.Aspects.Generators
{
    /// <summary>
    /// Generates exit aspect code.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class ExitAspectGenerator : AspectGenerator
    {
        public ExitAspectGenerator(MemberInfo memberInfo) : base(memberInfo) {}

        public IEnumerable<CodeStatement> GenerateForMethod()
        {
            var methodInfo = this.memberInfo as MethodInfo;
            if (methodInfo == null) throw new InvalidOperationException();

            yield return GenerateMethodEndStatement();

            var returnsValue = methodInfo.ReturnType != typeof (void);
            var aspectInvocationExpression = this.GenerateAspectInvocationForMethod(returnsValue);

            yield return new CodeExpressionStatement(aspectInvocationExpression);
        }

        public IEnumerable<CodeStatement> GenerateForPropertyGet()
        {
            yield return GenerateMethodEndStatement();

            var aspectInvocationExpression = this.GenerateAspectInvocationForProperty(true);

            yield return new CodeExpressionStatement(aspectInvocationExpression);
        }

        public IEnumerable<CodeStatement> GenerateForPropertySet()
        {
            yield return GenerateMethodEndStatement();

            var aspectInvocationExpression = this.GenerateAspectInvocationForProperty(false);

            yield return new CodeExpressionStatement(aspectInvocationExpression);
        }

        private static CodeStatement GenerateMethodEndStatement()
        {
            var initialValueExpression = new CodeMethodInvokeExpression(
                Constructs.NowExpression, "Subtract", Constructs.MethodStartExpression);

            return new CodeVariableDeclarationStatement(
                typeof (TimeSpan), VariableNames.Duration, initialValueExpression);
        }

        private CodeExpression GenerateAspectInvocationForMethod(bool returnsValue)
        {
            var parameterDictionaryExpression = this.GenerateParameterDictionaryExpression(returnsValue);
            var aspectExecutorExpression = new CodeTypeReferenceExpression(typeof (AspectExecutor));
            var getCurrentMethodExpression = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof (MethodBase)), "GetCurrentMethod");
            var returnValueExpression = new CodeDirectionExpression(
                FieldDirection.Ref, Constructs.BoxedReturnValueExpression);

            return new CodeMethodInvokeExpression(
                aspectExecutorExpression,
                "ExecuteExitAspects",
                Constructs.TargetFieldExpression,
                getCurrentMethodExpression,
                parameterDictionaryExpression,
                returnValueExpression,
                new CodeVariableReferenceExpression(VariableNames.Duration));
        }

        private CodeExpression GenerateAspectInvocationForProperty(bool isGet)
        {
            var parameterDictionaryExpression = this.GenerateParameterDictionaryExpression(isGet);
            var aspectExecutorExpression = new CodeTypeReferenceExpression(typeof (AspectExecutor));
            var getCurrentMethodExpression = new CodeSnippetExpression(
                string.Format("{0}.GetType().GetProperty(\"{1}\")", VariableNames.Target, this.memberInfo.Name));
            var returnValueExpression = new CodeDirectionExpression(
                FieldDirection.Ref, Constructs.BoxedReturnValueExpression);

            return new CodeMethodInvokeExpression(
                aspectExecutorExpression,
                "ExecuteExitAspects",
                Constructs.TargetFieldExpression,
                getCurrentMethodExpression,
                parameterDictionaryExpression,
                returnValueExpression,
                new CodeVariableReferenceExpression(VariableNames.Duration));
        }
    }
}