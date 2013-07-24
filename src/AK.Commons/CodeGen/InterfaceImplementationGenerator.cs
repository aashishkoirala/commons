/*******************************************************************************************************************************
 * AK.Commons.CodeGen.InterfaceImplementationGenerator
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

#region Namespace Imports

using System.Reflection;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

#endregion

namespace AK.Commons.CodeGen
{
    /// <summary>
    /// Provides methods to dynamically generate classes that implement given interfaces.
    /// 
    /// TODO: This class needs some work - it needs to be cleaned up; and also works for just one
    /// TODO: given scenario. Needs to be made more of a "component".
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class InterfaceImplementationGenerator
    {
        /// <summary>
        /// Dynamically generates classes that implement given interfaces.
        /// </summary>
        /// <param name="generatedClassesNamespace">Namespace to put generated classes in.</param>
        /// <param name="classNameFormat">
        /// Format string for class name. The placeholder {ContractType} will be replaced by 
        /// the interface full name (with periods replaced by underscores).
        /// </param>
        /// <param name="contractTypeList">List of interfaces to implement.</param>
        /// <param name="methodImplementationGeneratorAction">
        /// Action to take on each generated method body.
        /// </param>
        /// <param name="classAction">Class level actions to take.</param>
        /// <returns>
        /// Code generation output data structure that includes the source code 
        /// and assembly, among other things.
        /// </returns>
        public static CodeGenerationOutput Generate(
            string generatedClassesNamespace, 
            string classNameFormat, 
            IList<Type> contractTypeList,
            Action<MethodInfo, CodeMemberMethod> methodImplementationGeneratorAction,
            Action<Type, CodeTypeDeclaration> classAction = null)
        {
            var codeNamespace = new CodeNamespace(generatedClassesNamespace);

            contractTypeList.ForEach(
                contractType =>
                {
                    var codeTypeDeclaration =
                        GenerateClass(classNameFormat, contractType,
                                      methodImplementationGeneratorAction, classAction);

                    codeNamespace.Types.Add(codeTypeDeclaration);
                });

            var codeCompileUnit = new CodeCompileUnit();
            codeCompileUnit.Namespaces.Add(codeNamespace);
            AppDomain.CurrentDomain.GetAssemblies().ForEach(assembly => codeCompileUnit.ReferencedAssemblies.Add(assembly.Location));

            var codeGenerationOutput = new CodeGenerationOutput();

            using (var codeProvider = new CSharpCodeProvider())
            {
                codeGenerationOutput.CompilerResults = codeProvider.CompileAssemblyFromDom(
                    new CompilerParameters {GenerateInMemory = true}, codeCompileUnit);

                codeGenerationOutput.Success = !codeGenerationOutput.CompilerResults.Errors.HasErrors;

                if (codeGenerationOutput.Success)
                    codeGenerationOutput.Assembly = codeGenerationOutput.CompilerResults.CompiledAssembly;

                using (var stringWriter = new StringWriter())
                {
                    codeProvider.GenerateCodeFromCompileUnit(
                        codeCompileUnit, stringWriter,
                        new CodeGeneratorOptions
                        {
                            BlankLinesBetweenMembers = true,
                            BracingStyle = "C",
                            ElseOnClosing = true,
                            IndentString = "    ",
                            VerbatimOrder = true
                        });

                    codeGenerationOutput.SourceCode = stringWriter.ToString();
                }
            }

            return codeGenerationOutput;
        }

        private static CodeTypeDeclaration GenerateClass(
            string classNameFormat, 
            Type contractType, 
            Action<MethodInfo, CodeMemberMethod> methodImplementationGeneratorAction, 
            Action<Type, CodeTypeDeclaration> classAction)
        {
            Debug.Assert(contractType.FullName != null);

            var contractTypeName = contractType.FullName.Replace(".", "_");
            var className = classNameFormat.Replace("{ContractType}", contractTypeName);

            var codeTypeDeclaration = new CodeTypeDeclaration(className);
            codeTypeDeclaration.BaseTypes.Add(contractType);

            var codeMemberMethodDictionary = contractType.GetMethods()
                .ToDictionary(
                    methodInfo => new CodeMemberMethod
                    {
                        Attributes = MemberAttributes.Public,
                        Name = methodInfo.Name,
                        ReturnType = new CodeTypeReference(methodInfo.ReturnType)
                    });
                
            codeMemberMethodDictionary.ForEach((key, value) =>
            {
                if (methodImplementationGeneratorAction != null)
                    methodImplementationGeneratorAction(value, key);

                codeTypeDeclaration.Members.Add(key);
            });

            if (classAction != null)
                classAction(contractType, codeTypeDeclaration);

            return codeTypeDeclaration;
        }
    }
}