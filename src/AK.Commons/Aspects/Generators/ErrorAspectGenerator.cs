/*******************************************************************************************************************************
 * AK.Commons.Aspects.Generators.ErrorAspectGenerator
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
using System.Reflection;

#endregion

namespace AK.Commons.Aspects.Generators
{
    /// <summary>
    /// Generates error aspect code.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class ErrorAspectGenerator : AspectGenerator
    {
        public ErrorAspectGenerator(MemberInfo memberInfo) : base(memberInfo) {}

        public CodeStatement[] GenerateForMethod()
        {
            var methodInfo = this.memberInfo as MethodInfo;
            if (methodInfo == null) throw new InvalidOperationException();

            var returnsValue = methodInfo.ReturnType != typeof (void);

            var exceptionCopyStatement = new CodeVariableDeclarationStatement(
                typeof (Exception), VariableNames.ExceptionCopy,
                new CodeVariableReferenceExpression(VariableNames.Exception));
            var aspectInvocationExpression = this.GenerateAspectInvocationForMethod(returnsValue);

            var throwCopyStatement = new CodeThrowExceptionStatement(
                new CodeVariableReferenceExpression(VariableNames.ExceptionCopy));
            var throwOriginalStatement = new CodeThrowExceptionStatement();

            var compareCopyExpression = new CodeBinaryOperatorExpression(
                new CodeVariableReferenceExpression(VariableNames.Exception),
                CodeBinaryOperatorType.IdentityEquality,
                new CodeVariableReferenceExpression(VariableNames.ExceptionCopy));

            var compareAndThrowStatement = new CodeConditionStatement(
                compareCopyExpression, new CodeStatement[] {throwOriginalStatement},
                new CodeStatement[] {throwCopyStatement});

            var aspectInvokeAndThrowStatement = new CodeConditionStatement(
                aspectInvocationExpression, compareAndThrowStatement);

            return new CodeStatement[] {exceptionCopyStatement, aspectInvokeAndThrowStatement};
        }

        public CodeStatement[] GenerateForPropertyGet()
        {
            var exceptionCopyStatement = new CodeVariableDeclarationStatement(
                typeof(Exception), VariableNames.ExceptionCopy,
                new CodeVariableReferenceExpression(VariableNames.Exception));

            var aspectInvocationExpression = this.GenerateAspectInvocationForProperty(true);

            var throwCopyStatement = new CodeThrowExceptionStatement(
                new CodeVariableReferenceExpression(VariableNames.ExceptionCopy));
            var throwOriginalStatement = new CodeThrowExceptionStatement();

            var compareCopyExpression = new CodeBinaryOperatorExpression(
                new CodeVariableReferenceExpression(VariableNames.Exception),
                CodeBinaryOperatorType.IdentityEquality,
                new CodeVariableReferenceExpression(VariableNames.ExceptionCopy));

            var compareAndThrowStatement = new CodeConditionStatement(
                compareCopyExpression, new CodeStatement[] { throwOriginalStatement },
                new CodeStatement[] { throwCopyStatement });

            var aspectInvokeAndThrowStatement = new CodeConditionStatement(
                aspectInvocationExpression, compareAndThrowStatement);

            return new CodeStatement[] { exceptionCopyStatement, aspectInvokeAndThrowStatement };
        }

        public CodeStatement[] GenerateForPropertySet()
        {
            var exceptionCopyStatement = new CodeVariableDeclarationStatement(
                typeof(Exception), VariableNames.ExceptionCopy,
                new CodeVariableReferenceExpression(VariableNames.Exception));

            var aspectInvocationExpression = this.GenerateAspectInvocationForProperty(false);

            var throwCopyStatement = new CodeThrowExceptionStatement(
                new CodeVariableReferenceExpression(VariableNames.ExceptionCopy));
            var throwOriginalStatement = new CodeThrowExceptionStatement();

            var compareCopyExpression = new CodeBinaryOperatorExpression(
                new CodeVariableReferenceExpression(VariableNames.Exception),
                CodeBinaryOperatorType.IdentityEquality,
                new CodeVariableReferenceExpression(VariableNames.ExceptionCopy));

            var compareAndThrowStatement = new CodeConditionStatement(
                compareCopyExpression, new CodeStatement[] { throwOriginalStatement },
                new CodeStatement[] { throwCopyStatement });

            var aspectInvokeAndThrowStatement = new CodeConditionStatement(
                aspectInvocationExpression, compareAndThrowStatement);

            return new CodeStatement[] { exceptionCopyStatement, aspectInvokeAndThrowStatement };
        }

        private CodeExpression GenerateAspectInvocationForMethod(bool returnsValue)
        {
            var parameterDictionaryExpression = this.GenerateParameterDictionaryExpression(returnsValue);
            var aspectExecutorExpression = new CodeTypeReferenceExpression(typeof (AspectExecutor));
            var getCurrentMethodExpression = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof (MethodBase)), "GetCurrentMethod");
            var exceptionExpression = new CodeVariableReferenceExpression(VariableNames.ExceptionCopy);
            var exceptionParameterExpression = new CodeDirectionExpression(FieldDirection.Ref, exceptionExpression);
            var returnValueExpression = new CodeDirectionExpression(
                FieldDirection.Ref, Constructs.BoxedReturnValueExpression);

            return new CodeMethodInvokeExpression(
                aspectExecutorExpression,
                "ExecuteErrorAspects",
                Constructs.TargetFieldExpression,
                getCurrentMethodExpression,
                parameterDictionaryExpression,
                exceptionParameterExpression,
                returnValueExpression);
        }

        private CodeExpression GenerateAspectInvocationForProperty(bool isGet)
        {
            var parameterDictionaryExpression = this.GenerateParameterDictionaryExpression(isGet);
            var aspectExecutorExpression = new CodeTypeReferenceExpression(typeof (AspectExecutor));
            var getCurrentMethodExpression = new CodeSnippetExpression(
                string.Format("{0}.GetType().GetProperty(\"{1}\")", VariableNames.Target, this.memberInfo.Name));
            var exceptionExpression = new CodeVariableReferenceExpression(VariableNames.ExceptionCopy);
            var exceptionParameterExpression = new CodeDirectionExpression(FieldDirection.Ref, exceptionExpression);
            var returnValueExpression = new CodeDirectionExpression(
                FieldDirection.Ref, Constructs.BoxedReturnValueExpression);

            return new CodeMethodInvokeExpression(
                aspectExecutorExpression,
                "ExecuteErrorAspects",
                Constructs.TargetFieldExpression,
                getCurrentMethodExpression,
                parameterDictionaryExpression,
                exceptionParameterExpression,
                returnValueExpression);
        }
    }
}