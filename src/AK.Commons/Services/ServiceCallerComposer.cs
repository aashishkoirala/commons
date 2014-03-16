/*******************************************************************************************************************************
 * AK.Commons.Services.ServiceCallerComposer
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

using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

#endregion

namespace AK.Commons.Services
{
    /// <summary>
    /// Registers WCF clients with MEF.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class ServiceCallerComposer
    {
        private const string TypeName = "C___ServiceContractExports";
        private const string NamespaceName = "C___ServiceContractExportNamespace";
        private const string TypeFullName = NamespaceName + "." + TypeName;

        /// <summary>
        /// Registers in MEFs exports for all WCF contracts in the given assemblies. The exports are registered so
        /// as to call ServiceCallerFactory.Create.
        /// </summary>
        /// <param name="container">MEF composition container. Must be made out of an AggregateCatalog.</param>
        /// <param name="assemblies">Assemblies to scan.</param>
        public static void Compose(CompositionContainer container, params Assembly[] assemblies)
        {
            if (!(container.Catalog is AggregateCatalog))
                throw new NotSupportedException("The composition container must have an AggregateCatalog.");

            var contractTypes = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsInterface)
                .Where(x => x.GetCustomAttributes(typeof (ServiceContractAttribute), true).Any())
                .Distinct()
                .ToArray();

            if (!contractTypes.Any()) return;

            var nonShared = new CodePropertyReferenceExpression(
                new CodeTypeReferenceExpression(typeof (CreationPolicy)),
                CreationPolicy.NonShared.ToString());

            var creationPolicy =
                new CodeAttributeDeclaration(
                    new CodeTypeReference(typeof (PartCreationPolicyAttribute)),
                    new CodeAttributeArgument(nonShared));

            var typeDeclaration = new CodeTypeDeclaration(TypeName)
            {
                TypeAttributes = TypeAttributes.Abstract | TypeAttributes.Public
            };
            typeDeclaration.CustomAttributes.Add(creationPolicy);

            foreach (var contractType in contractTypes)
                GenerateExport(contractType, typeDeclaration);

            var assembly = Compile(typeDeclaration, assemblies);
            var exportType = assembly.GetType(TypeFullName);

            ((AggregateCatalog) container.Catalog).Catalogs.Add(new TypeCatalog(exportType));
        }

        private static Assembly Compile(CodeTypeDeclaration typeDeclaration, IEnumerable<Assembly> assemblies)
        {
            var namespaceDeclaration = new CodeNamespace(NamespaceName);
            namespaceDeclaration.Types.Add(typeDeclaration);

            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(namespaceDeclaration);
            compileUnit.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            compileUnit.ReferencedAssemblies.Add(typeof (ExportAttribute).Assembly.Location);
            compileUnit.ReferencedAssemblies.AddRange(assemblies.Select(x => x.Location).ToArray());

            Assembly assembly;

            using (var codeProvider = new CSharpCodeProvider())
            {
                var results = codeProvider.CompileAssemblyFromDom(
                    new CompilerParameters
                    {
                        GenerateInMemory = true
                    }, compileUnit);

                if (results.Errors.HasErrors) throw new ApplicationException("Cannot compile generated code.");
                assembly = results.CompiledAssembly;
            }

            return assembly;
        }

        private static void GenerateExport(Type contractType, CodeTypeDeclaration typeDeclaration)
        {
            var contractTypeName = contractType.FullName != null
                                       ? contractType.FullName.Replace('.', '_')
                                       : Guid.NewGuid().ToString().Replace('-', '_');

            var propertyName = string.Format("Exported_{0}_{1}", contractTypeName,
                                             Guid.NewGuid().ToString().Replace('-', '_'));

            var serviceCallerType = typeof (IServiceCaller<>).MakeGenericType(contractType);

            // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
            var propertyDeclaration = new CodeMemberProperty
            {
                Name = propertyName,
                HasGet = true,
                HasSet = false,
                Attributes = MemberAttributes.Public | MemberAttributes.Static,
                Type = new CodeTypeReference(serviceCallerType)
            };
            // ReSharper restore BitwiseOperatorOnEnumWithoutFlags

            var methodReference = new CodeMethodReferenceExpression(
                new CodeTypeReferenceExpression(typeof (ServiceCallerFactory)), "Create");
            methodReference.TypeArguments.Add(contractType);

            var methodInvocation = new CodeMethodInvokeExpression(methodReference);

            propertyDeclaration.GetStatements.Add(new CodeMethodReturnStatement(methodInvocation));

            var export = new CodeAttributeDeclaration(
                new CodeTypeReference(typeof (ExportAttribute)),
                new CodeAttributeArgument(new CodeTypeOfExpression(serviceCallerType)));

            propertyDeclaration.CustomAttributes.Add(export);

            typeDeclaration.Members.Add(propertyDeclaration);
        }
    }
}