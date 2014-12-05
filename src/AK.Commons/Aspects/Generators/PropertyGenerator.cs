/*******************************************************************************************************************************
 * AK.Commons.Aspects.Generators.PropertyGenerator
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

using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace AK.Commons.Aspects.Generators
{
    /// <summary>
    /// Generates a wrapped property based on a given property. The wrapped property consists of aspect code and calls to the
    /// original property.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class PropertyGenerator
    {
        private readonly PropertyInfo contractProperty;
        private readonly AspectGenerator aspectGenerator;

        private CodeMemberProperty propertyDeclaration;
        private bool isIndexer;
        private CodeStatement[] getStatements;
        private CodeStatement[] setStatements;

        public PropertyGenerator(
            PropertyInfo contractProperty,
            AspectGenerator aspectGenerator)
        {
            this.contractProperty = contractProperty;
            this.aspectGenerator = aspectGenerator;
        }

        public CodeMemberProperty Generate()
        {
            this.propertyDeclaration = new CodeMemberProperty
            {
                Name = this.contractProperty.Name,
                Type = new CodeTypeReference(this.contractProperty.PropertyType),
                Attributes = MemberAttributes.Public,
                HasGet = this.contractProperty.CanRead,
                HasSet = this.contractProperty.CanWrite
            };
            this.isIndexer = this.contractProperty.Name == "Item";

            this.AssignGetAndSetStatements();
            this.AssignGetBlockWithAspectCode();
            this.AssignSetBlockWithAspectCode();

            return this.propertyDeclaration;
        }

        private void AssignGetAndSetStatements()
        {
            if (this.isIndexer)
            {
                var indexParameters = this.contractProperty.GetIndexParameters();

                foreach (var parameterDeclaration in indexParameters
                    .Select(x => new CodeParameterDeclarationExpression(x.ParameterType, x.Name)))
                {
                    this.propertyDeclaration.Parameters.Add(parameterDeclaration);
                }

                var parameterDeclarations = indexParameters
                    .Select(x => new CodeVariableReferenceExpression(x.Name))
                    .Cast<CodeExpression>()
                    .ToArray();

                if (this.propertyDeclaration.HasGet)
                    this.getStatements = GetStatementsForIndexerGetBlock(parameterDeclarations).ToArray();

                if (this.propertyDeclaration.HasSet)
                    this.setStatements = GetStatementsForIndexerSetBlock(parameterDeclarations).ToArray();

                return;
            }

            if (this.propertyDeclaration.HasGet)
                this.getStatements = this.GetStatementsForPropertyGetBlock().ToArray();

            if (this.propertyDeclaration.HasSet)
                this.setStatements = this.GetStatementsForPropertySetBlock().ToArray();
        }

        private void AssignGetBlockWithAspectCode()
        {
            if (!this.propertyDeclaration.HasGet) return;

            var entryStatements = this.aspectGenerator.Entry.GenerateForPropertyGet().ToArray();
            var exitStatements = this.aspectGenerator.Exit.GenerateForPropertyGet().ToArray();
            var errorStatements = this.aspectGenerator.Error.GenerateForPropertyGet().ToArray();

            var catchClause = Constructs.CatchClause;
            catchClause.Statements.AddRange(errorStatements);

            var tryCatchFinallyBlock = new CodeTryCatchFinallyStatement(
                this.getStatements, catchClause.AsArray(), exitStatements);

            var returnValueDefaultValueExpression = new CodeDefaultValueExpression(
                new CodeTypeReference(this.contractProperty.PropertyType));

            var returnValueDeclarationStatement = new CodeVariableDeclarationStatement(
                this.contractProperty.PropertyType,
                VariableNames.ReturnValue,
                returnValueDefaultValueExpression);

            var boxingSnippet = string.Format(
                "object {0} = {1};",
                VariableNames.BoxedReturnValue,
                VariableNames.ReturnValue);

            var boxingStatement = new CodeSnippetStatement(boxingSnippet);

            var returnValueExpression = new CodeCastExpression(
                this.contractProperty.PropertyType, Constructs.BoxedReturnValueExpression);

            var returnValueStatement = new CodeMethodReturnStatement(returnValueExpression);

            this.propertyDeclaration.GetStatements.Add(returnValueDeclarationStatement);
            this.propertyDeclaration.GetStatements.Add(boxingStatement);
            this.propertyDeclaration.GetStatements.AddRange(entryStatements);
            this.propertyDeclaration.GetStatements.Add(tryCatchFinallyBlock);
            this.propertyDeclaration.GetStatements.Add(returnValueStatement);
        }

        private void AssignSetBlockWithAspectCode()
        {
            if (!this.propertyDeclaration.HasSet) return;

            var entryStatements = this.aspectGenerator.Entry.GenerateForPropertySet().ToArray();
            var exitStatements = this.aspectGenerator.Exit.GenerateForPropertySet().ToArray();
            var errorStatements = this.aspectGenerator.Error.GenerateForPropertySet().ToArray();

            var catchClause = Constructs.CatchClause;
            catchClause.Statements.AddRange(errorStatements);

            var tryCatchFinallyBlock = new CodeTryCatchFinallyStatement(
                this.setStatements, catchClause.AsArray(), exitStatements);

            var boxingSnippet = string.Format(
                "object {0} = null;", VariableNames.BoxedReturnValue);
            var boxingStatement = new CodeSnippetStatement(boxingSnippet);

            this.propertyDeclaration.SetStatements.Add(boxingStatement);
            this.propertyDeclaration.SetStatements.AddRange(entryStatements);
            this.propertyDeclaration.SetStatements.Add(tryCatchFinallyBlock);
        }

        private IEnumerable<CodeStatement> GetStatementsForPropertyGetBlock()
        {
            yield return new CodeAssignStatement(
                Constructs.ReturnValueExpression,
                new CodePropertyReferenceExpression(Constructs.TargetFieldExpression, this.contractProperty.Name));

            yield return
                new CodeAssignStatement(Constructs.BoxedReturnValueExpression, Constructs.ReturnValueExpression);
        }

        private IEnumerable<CodeStatement> GetStatementsForPropertySetBlock()
        {
            yield return new CodeAssignStatement(
                new CodePropertyReferenceExpression(Constructs.TargetFieldExpression, this.contractProperty.Name),
                Constructs.ValueExpression);
        }

        private static IEnumerable<CodeStatement> GetStatementsForIndexerGetBlock(CodeExpression[] parameterDeclarations)
        {
            yield return new CodeAssignStatement(
                Constructs.ReturnValueExpression,
                new CodeIndexerExpression(Constructs.TargetFieldExpression, parameterDeclarations));

            yield return
                new CodeAssignStatement(Constructs.BoxedReturnValueExpression, Constructs.ReturnValueExpression);
        }

        private static IEnumerable<CodeStatement> GetStatementsForIndexerSetBlock(CodeExpression[] parameterDeclarations)
        {
            yield return new CodeAssignStatement(
                new CodeIndexerExpression(Constructs.TargetFieldExpression, parameterDeclarations),
                Constructs.ValueExpression);
        }
    }
}