/*******************************************************************************************************************************
 * AK.Commons.Aspects.Generators.MethodGenerator
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
using System.Linq;
using System.Reflection;

#endregion

namespace AK.Commons.Aspects.Generators
{
    /// <summary>
    /// Generates a wrapped method based on a given method. The wrapped method consists of aspect code and a call to the
    /// original method. 
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class MethodGenerator
    {
        private readonly MethodInfo contractMethod;
        private readonly MethodInfo implementationMethod;
        private readonly AspectGenerator aspectGenerator;

        private CodeMemberMethod methodDeclaration;
        private CodeMethodInvokeExpression methodInvocation;
        private bool returnsValue;

        public MethodGenerator(
            MethodInfo contractMethod,
            MethodInfo implementationMethod,
            AspectGenerator aspectGenerator)
        {
            this.contractMethod = contractMethod;
            this.implementationMethod = implementationMethod;
            this.aspectGenerator = aspectGenerator;
        }

        public CodeMemberMethod Generate()
        {
            this.returnsValue = this.contractMethod.ReturnType != typeof (void);

            this.methodDeclaration = new CodeMemberMethod
            {
                Name = this.contractMethod.Name,
                ReturnType = new CodeTypeReference(this.contractMethod.ReturnType),
                Attributes = MemberAttributes.Public
            };

            this.methodInvocation = new CodeMethodInvokeExpression(
                Constructs.TargetFieldExpression,
                this.contractMethod.Name);

            this.AssignParameters();
            this.AssignTypeParameters();
            this.AssignReturnValueDeclaration();
            this.AssignBodyWithAspectCode();

            if (this.returnsValue)
            {
                var castExpression = new CodeCastExpression(
                    this.contractMethod.ReturnType, Constructs.BoxedReturnValueExpression);
                var returnValueStatement = new CodeMethodReturnStatement(castExpression);
                this.methodDeclaration.Statements.Add(returnValueStatement);
            }

            this.AssignAspectAttributes();

            return this.methodDeclaration;
        }

        private void AssignParameters()
        {
            var parameterDeclarations = this.GetParameterDeclarations();

            foreach (var parameterDeclaration in parameterDeclarations)
            {
                this.methodDeclaration.Parameters.Add(parameterDeclaration);

                if (parameterDeclaration.Direction == FieldDirection.Out)
                {
                    var parameterExpression = new CodeVariableReferenceExpression(parameterDeclaration.Name);
                    var defaultValueExpression = new CodeDefaultValueExpression(parameterDeclaration.Type);
                    var defaultValueStatement = new CodeAssignStatement(parameterExpression,
                                                                        defaultValueExpression);

                    this.methodDeclaration.Statements.Add(defaultValueStatement);
                }

                var prefix = parameterDeclaration.Direction == FieldDirection.Out
                                 ? "out "
                                 : (parameterDeclaration.Direction == FieldDirection.Ref ? "ref " : "");

                var parameterPassExpression = new CodeVariableReferenceExpression(prefix + parameterDeclaration.Name);
                this.methodInvocation.Parameters.Add(parameterPassExpression);
            }
        }

        private IEnumerable<CodeParameterDeclarationExpression> GetParameterDeclarations()
        {
            Func<Type, CodeTypeReference> parameterTypeFunc =
                x =>
                new CodeTypeReference(x.IsByRef ? x.GetElementType() : x);

            Func<ParameterInfo, FieldDirection> directionFunc =
                x =>
                !x.ParameterType.IsByRef
                    ? FieldDirection.In
                    : (x.IsOut ? FieldDirection.Out : FieldDirection.Ref);

            return this.contractMethod.GetParameters()
                .Select(x => new CodeParameterDeclarationExpression
                {
                    Name = x.Name,
                    Type = parameterTypeFunc(x.ParameterType),
                    Direction = directionFunc(x)
                });
        }

        private void AssignBodyWithAspectCode()
        {
            var entryStatements = this.aspectGenerator.Entry.GenerateForMethod().ToArray();
            var exitStatements = this.aspectGenerator.Exit.GenerateForMethod().ToArray();
            var errorStatements = this.aspectGenerator.Error.GenerateForMethod().ToArray();

            this.methodDeclaration.Statements.AddRange(entryStatements);

            var methodInvocationStatement = (CodeStatement) new CodeExpressionStatement(this.methodInvocation);
            var methodInvocationAssignmentStatement =
                new CodeAssignStatement(Constructs.ReturnValueExpression, this.methodInvocation);

            CodeStatement[] tryBodyStatements;
            if (this.returnsValue)
            {
                var tryBodyStatement = methodInvocationAssignmentStatement;
                var boxingStatement = new CodeSnippetStatement(
                    string.Format("{0} = {1};", VariableNames.BoxedReturnValue, VariableNames.ReturnValue));

                tryBodyStatements = new CodeStatement[] {tryBodyStatement, boxingStatement};
            }
            else
            {
                var tryBodyStatement = methodInvocationStatement;
                tryBodyStatements = tryBodyStatement.AsArray();
            }

            var catchClause = Constructs.CatchClause;
            catchClause.Statements.AddRange(errorStatements);

            var tryCatchFinallyBlock = new CodeTryCatchFinallyStatement(
                tryBodyStatements, catchClause.AsArray(), exitStatements);

            this.methodDeclaration.Statements.Add(tryCatchFinallyBlock);
        }

        private void AssignReturnValueDeclaration()
        {
            string boxingSnippet;
            if (this.returnsValue)
            {
                var returnValueDefaultValueExpression = new CodeDefaultValueExpression(
                    new CodeTypeReference(this.contractMethod.ReturnType));

                var returnValueVariableDeclaration = new CodeVariableDeclarationStatement(
                    this.contractMethod.ReturnType,
                    VariableNames.ReturnValue,
                    returnValueDefaultValueExpression);

                this.methodDeclaration.Statements.Add(returnValueVariableDeclaration);

                boxingSnippet = string.Format(
                    "object {0} = {1};", VariableNames.BoxedReturnValue, VariableNames.ReturnValue);
            }
            else boxingSnippet = string.Format("object {0} = null;", VariableNames.BoxedReturnValue);

            var boxingStatement = new CodeSnippetStatement(boxingSnippet);
            this.methodDeclaration.Statements.Add(boxingStatement);
        }

        private void AssignTypeParameters()
        {
            foreach (var typeParameter in this.contractMethod.GetGenericArguments())
            {
                this.methodDeclaration.TypeParameters.Add(new CodeTypeParameter(typeParameter.Name));
                this.methodInvocation.Method.TypeArguments.Add(typeParameter);
            }
        }

        private void AssignAspectAttributes()
        {
            if (this.implementationMethod == null) return;

            var aspectAttributeDataList =
                this.implementationMethod.GetCustomAttributesData()
                    .Where(x => x.AttributeType.GetInterfaces().Contains(typeof (IAspect)));

            foreach (var attributeData in aspectAttributeDataList)
            {
                var attributeParameters = new List<CodeAttributeArgument>();

                AssignAspectAttributeConstructorArguments(attributeData, attributeParameters);
                AssignAspectAttributeNamedArguments(attributeData, attributeParameters);

                var attributeDeclaration = new CodeAttributeDeclaration(
                    new CodeTypeReference(attributeData.AttributeType),
                    attributeParameters.ToArray());

                this.methodDeclaration.CustomAttributes.Add(attributeDeclaration);
            }
        }

        private static void AssignAspectAttributeConstructorArguments(
            CustomAttributeData attributeData,
            ICollection<CodeAttributeArgument> attributeParameters)
        {
            foreach (var argument in attributeData.ConstructorArguments)
            {
                CodeExpression ce;

                if (argument.ArgumentType.IsPrimitive || argument.ArgumentType == typeof (string))
                    ce = new CodePrimitiveExpression(argument.Value);

                else if (argument.ArgumentType == typeof (Type))
                    ce = new CodeTypeOfExpression((Type) argument.Value);

                else continue;

                attributeParameters.Add(new CodeAttributeArgument(ce));
            }
        }

        private static void AssignAspectAttributeNamedArguments(
            CustomAttributeData attributeData,
            ICollection<CodeAttributeArgument> attributeParameters)
        {
            if (attributeData.NamedArguments == null) return;

            foreach (var namedArgument in attributeData.NamedArguments)
            {
                var argument = namedArgument.TypedValue;
                CodeExpression ce;

                if (argument.ArgumentType.IsPrimitive || argument.ArgumentType == typeof (string))
                    ce = new CodePrimitiveExpression(argument.Value);

                else if (argument.ArgumentType == typeof (Type))
                    ce = new CodeTypeOfExpression((Type) argument.Value);

                else continue;

                attributeParameters.Add(new CodeAttributeArgument(namedArgument.MemberName, ce));
            }
        }
    }
}