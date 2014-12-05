/*******************************************************************************************************************************
 * AK.Commons.Aspects.Generators.AspectedTypeGenerator
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
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

#endregion

namespace AK.Commons.Aspects.Generators
{
    /// <summary>
    /// Generates a wrapping type with aspect code based on a given contract type and implementation type. The contract type
    /// should be an interface, and the implementation type is expected to be decorated with aspect attributes.
    /// </summary>
    /// <author>Aashish Koirala</author>
    internal class AspectedTypeGenerator
    {
        private readonly Type contractType;
        private readonly Type implementationType;

        private CodeTypeDeclaration typeDeclaration;

        public AspectedTypeGenerator(Type contractType, Type implementationType)
        {
            this.contractType = contractType;
            this.implementationType = implementationType;
        }

        public CodeCompileUnit Generate()
        {
            var typeName = string.Format("AopProxy_{0}", Guid.NewGuid()).Replace('-', '_');

            this.typeDeclaration = new CodeTypeDeclaration(typeName);
            this.typeDeclaration.BaseTypes.Add(new CodeTypeReference(this.contractType));

            var targetFieldDeclaration =
                new CodeMemberField(this.contractType, VariableNames.Target) {Attributes = MemberAttributes.Private};
            this.typeDeclaration.Members.Add(targetFieldDeclaration);

            this.AssignGeneratedConstructor();
            this.AssignGeneratedMethods();
            this.AssignGeneratedProperties();
            this.AssignGeneratedEvents();
            this.AssignMefAttributes();

            return this.CreateCodeCompileUnit();
        }

        private void AssignGeneratedConstructor()
        {
            var constructor = new CodeConstructor {Attributes = MemberAttributes.Public};

            var targetParameter = new CodeParameterDeclarationExpression(this.contractType, VariableNames.Target);
            targetParameter.CustomAttributes.Add(
                new CodeAttributeDeclaration(new CodeTypeReference(typeof (ImportAttribute))));
            constructor.Parameters.Add(targetParameter);

            var thisTargetExpression = new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), VariableNames.Target);

            constructor.Statements.Add(
                new CodeAssignStatement(thisTargetExpression, Constructs.TargetFieldExpression));

            constructor.CustomAttributes.Add(
                new CodeAttributeDeclaration(new CodeTypeReference(typeof (ImportingConstructorAttribute))));

            this.typeDeclaration.Members.Add(constructor);
        }

        private void AssignGeneratedMethods()
        {
            var generatedMethods = (from contractMethod in this.GetMethodsToGenerate()
                                    let implementationMethod = this.GetMatchingImplementationMethod(contractMethod)
                                    let aspectGenerator = new AspectGenerator(implementationMethod)
                                    let methodGenerator =
                                        new MethodGenerator(contractMethod, implementationMethod, aspectGenerator)
                                    select methodGenerator.Generate())
                .Cast<CodeTypeMember>()
                .ToArray();

            this.typeDeclaration.Members.AddRange(generatedMethods);
        }

        private void AssignGeneratedProperties()
        {
            var propertiesToGenerate = this.contractType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var generatedProperties = (from contractProperty in propertiesToGenerate
                                       let implementationProperty =
                                           this.implementationType.GetProperty(contractProperty.Name)
                                       let aspectGenerator = new AspectGenerator(implementationProperty)
                                       let propertyGenerator =
                                           new PropertyGenerator(contractProperty, aspectGenerator)
                                       select propertyGenerator.Generate())
                .Cast<CodeTypeMember>()
                .ToArray();

            this.typeDeclaration.Members.AddRange(generatedProperties);
        }

        private void AssignGeneratedEvents()
        {
            var eventsToGenerate = this.contractType.GetEvents(BindingFlags.Public | BindingFlags.Instance);
            foreach (var generatedEvent in eventsToGenerate.Select(GenerateEvent))
                this.typeDeclaration.Members.Add(generatedEvent);
        }

        private void AssignMefAttributes()
        {
            var aspectedContractType = typeof (IAspected<>).MakeGenericType(this.contractType);
            this.typeDeclaration.BaseTypes.Add(new CodeTypeReference(aspectedContractType));

            var exportAttributeDeclaration =
                new CodeAttributeDeclaration(new CodeTypeReference(typeof (ExportAttribute)));
            exportAttributeDeclaration.Arguments.Add(
                new CodeAttributeArgument(new CodeTypeOfExpression(aspectedContractType)));

            var partCreationPolicyAttribute =
                this.implementationType.GetCustomAttribute<PartCreationPolicyAttribute>() ??
                new PartCreationPolicyAttribute(CreationPolicy.Any);

            var partCreationPolicyAttributeDeclaration =
                new CodeAttributeDeclaration(new CodeTypeReference(typeof (PartCreationPolicyAttribute)));

            var creationPolicyExpression = new CodeSnippetExpression(
                string.Format("{0}.{1}", typeof (CreationPolicy).FullName,
                              partCreationPolicyAttribute.CreationPolicy));

            var partCreationPolicyAttributeArgument = new CodeAttributeArgument(creationPolicyExpression);

            partCreationPolicyAttributeDeclaration.Arguments.Add(
                partCreationPolicyAttributeArgument);

            this.typeDeclaration.CustomAttributes.Add(exportAttributeDeclaration);
            this.typeDeclaration.CustomAttributes.Add(partCreationPolicyAttributeDeclaration);
        }

        private CodeCompileUnit CreateCodeCompileUnit()
        {
            var namespaceInCompileUnit = new CodeNamespace("AopProxies");
            namespaceInCompileUnit.Types.Add(this.typeDeclaration);
            namespaceInCompileUnit.Imports.Add(new CodeNamespaceImport("System"));
            namespaceInCompileUnit.Imports.Add(new CodeNamespaceImport("System.Linq"));

            var codeCompileUnit = new CodeCompileUnit();
            codeCompileUnit.Namespaces.Add(namespaceInCompileUnit);

            var executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            codeCompileUnit.ReferencedAssemblies.Add(executingAssemblyLocation);

            foreach (var assemblyLocation in AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic)
                .Select(x => x.Location)
                .Where(x => x != executingAssemblyLocation)
                .Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                codeCompileUnit.ReferencedAssemblies.Add(assemblyLocation);
            }

            return codeCompileUnit;
        }

        private static CodeTypeMember GenerateEvent(EventInfo contractEvent)
        {
            var eventHandlerType = contractEvent.EventHandlerType;
            var eventHandlerSnippet = eventHandlerType.FullName;

            if (eventHandlerType.IsGenericType)
            {
                eventHandlerSnippet = eventHandlerType.GetGenericTypeDefinition().FullName;
                Debug.Assert(eventHandlerSnippet != null);

                eventHandlerSnippet = eventHandlerSnippet.Split('`')[0];
                var sb = new StringBuilder(eventHandlerSnippet);
                sb.Append("<");

                foreach (var typeArgument in eventHandlerType.GenericTypeArguments)
                {
                    sb.Append(typeArgument.FullName);
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(">");

                eventHandlerSnippet = sb.ToString();
            }

            var eventSnippet =
                "public event {1} {0} {{ add {{ target.{0} += value; }} remove {{ target.{0} -= value; }} }}";
            eventSnippet = string.Format(eventSnippet, contractEvent.Name, eventHandlerSnippet);

            return new CodeSnippetTypeMember(eventSnippet);
        }

        private IEnumerable<MethodInfo> GetMethodsToGenerate()
        {
            return this.contractType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => !x.Name.StartsWith("get_") && !x.Name.StartsWith("set_") &&
                            !x.Name.StartsWith("add_") && !x.Name.StartsWith("remove_"));
        }

        private MethodInfo GetMatchingImplementationMethod(MethodBase contractMethod)
        {
            var types = contractMethod.GetParameters()
                .Select(x => x.ParameterType)
                .ToArray();

            var implementationMethod = this.implementationType.GetMethod(contractMethod.Name, types);

            if (implementationMethod != null) return implementationMethod;

            var candidateMethods = this.implementationType.GetMethods()
                .Where(x => x.Name == contractMethod.Name)
                .Where(x => x.GetParameters().Length == contractMethod.GetParameters().Length);

            var contractMethodGenericArgumentCount = contractMethod.GetGenericArguments().Length;

            implementationMethod = (from candidateMethod in candidateMethods
                                    let candidateMethodGenericArgumentCount =
                                        candidateMethod.GetGenericArguments().Length
                                    where contractMethodGenericArgumentCount == candidateMethodGenericArgumentCount
                                    select candidateMethod).FirstOrDefault();

            if (implementationMethod != null) return implementationMethod;

            throw new Exception(string.Format("Cannot process method {0}", contractMethod.Name));
        }
    }
}