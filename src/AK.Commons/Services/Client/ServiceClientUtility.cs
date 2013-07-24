/*******************************************************************************************************************************
 * AK.Commons.Services
 * 
 * THIS NAMESPACE IS UNDER DEVELOPMENT.
 * 
 * TODO: Build WCF library within AK.Common.Services.
 * 
 * Copyright © 2013 Aashish Koirala <http://aashishkoirala.github.io>
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

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using AK.Commons.CodeGen;
using AK.Commons.Configuration;

namespace AK.Commons.Services.Client
{
    public static class ServiceClientUtility
    {
        public static Assembly GenerateServiceClients()
        {
            // ReSharper disable PossibleNullReferenceException

            var contractTypeList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsInterface) 
                .Where(x => x.FullName.StartsWith("AK.")) // TODO: Find better filter
                .Where(x => x.GetCustomAttributes(typeof(ServiceContractAttribute), true).Any())
                .ToList();

            // ReSharper restore PossibleNullReferenceException

            var serviceClientGenerationOutput = GenerateServiceClients("AK.GeneratedCode.ServiceClients", contractTypeList);

            return serviceClientGenerationOutput.CodeGenerationOutput.Assembly;
        }

        public static ServiceEndpoint CreateEndpoint<TContract>()
        {
            var serviceConfiguration = AppEnvironment.Config.GetServiceConfiguration();
            var clientConfiguration = serviceConfiguration.GetClientConfiguration<TContract>();

            var contractDescription = ContractDescription.GetContract(typeof (TContract));
            var binding = clientConfiguration.BindingConfiguration.Instantiate<Binding>();
            var endpointAddress = new EndpointAddress(clientConfiguration.EndpointAddressUri);

            return new ServiceEndpoint(contractDescription, binding, endpointAddress);
        }

        public static ServiceConfiguration GetServiceConfiguration(this IAppConfig config)
        {
            return new ServiceConfiguration(config);
        }

        private static ServiceClientGenerationOutput GenerateServiceClients(
            string generatedClassesNamespace, 
            IList<Type> contractTypeList)
        {
            var contractToImplementationNameDictionary = new Dictionary<Type, string>();

            var codeGenerationOutput = InterfaceImplementationGenerator.Generate(
                generatedClassesNamespace, "Client_{ContractType}", contractTypeList,
                (methodInfo, codeMemberMethod) =>
                {
                    var snippetBuilder = new StringBuilder("this.Channel.").Append(methodInfo.Name).Append("(");

                    var parameterInfoList = methodInfo.GetParameters();
                    var parameterIndex = 0;
                    foreach (var parameterInfo in parameterInfoList)
                    {
                        codeMemberMethod.Parameters.Add(
                            new CodeParameterDeclarationExpression(parameterInfo.ParameterType, parameterInfo.Name));

                        var separator = parameterIndex == parameterInfoList.Length - 1 ? string.Empty : ", ";
                        snippetBuilder.Append(parameterInfo.Name).Append(separator);

                        parameterIndex++;
                    }

                    if (methodInfo.ReturnType != typeof (void))
                        snippetBuilder.Insert(0, "return ");

                    snippetBuilder.Append(");");

                    codeMemberMethod.Statements.Add(new CodeSnippetStatement(snippetBuilder.ToString()));
                },
                (contractType, codeTypeDeclaration) =>
                {
                    codeTypeDeclaration.BaseTypes.Insert(0, new CodeTypeReference(typeof (ClientBase<>).MakeGenericType(contractType)));
                    
                    var className = generatedClassesNamespace + "." + codeTypeDeclaration.Name;
                    contractToImplementationNameDictionary[contractType] = className;

                    var exportAttributeDeclaration =
                        new CodeAttributeDeclaration(
                        new CodeTypeReference(typeof (ExportAttribute)), 
                        new CodeAttributeArgument(new CodeTypeOfExpression(contractType)));

                    var serviceClientAttributeDeclaration =
                        new CodeAttributeDeclaration(new CodeTypeReference(typeof (ServiceMetadataAttribute)), new CodeAttributeArgument(new CodeSnippetExpression("IsClient = true")));

                    codeTypeDeclaration.CustomAttributes.Add(exportAttributeDeclaration);
                    codeTypeDeclaration.CustomAttributes.Add(serviceClientAttributeDeclaration);

                    var endpointCreationSnippet = string.Format("{0}.CreateEndpoint<{1}>()", typeof(ServiceClientUtility).FullName, contractType.FullName);

                    var codeConstructor = new CodeConstructor {Attributes = MemberAttributes.Public};
                    codeConstructor.BaseConstructorArgs.Add(new CodeSnippetExpression(endpointCreationSnippet));

                    codeTypeDeclaration.Members.Add(codeConstructor);
                });

            if (!codeGenerationOutput.Success)
            {
                var errorText = codeGenerationOutput.CompilerResults.Errors
                    .OfType<CompilerError>()
                    .Select(x => x.ErrorText)
                    .Aggregate((error1, error2) => error1 + " " + error2);

                throw new Exception(errorText + "\r\n" + codeGenerationOutput.SourceCode);
            }

            var contractToImplementationDictionary =
                contractToImplementationNameDictionary
                .Select(pair => new KeyValuePair<Type, Type>(pair.Key, codeGenerationOutput.Assembly.GetType(pair.Value)))
                .ToDictionary(x => x.Key, x => x.Value);

            return new ServiceClientGenerationOutput
            {
                CodeGenerationOutput = codeGenerationOutput,
                ContractToImplementationDictionary = contractToImplementationDictionary
            };
        }

        private class ServiceClientGenerationOutput
        {
            public CodeGenerationOutput CodeGenerationOutput { get; set; }
            
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public IDictionary<Type, Type> ContractToImplementationDictionary { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }

    }
}