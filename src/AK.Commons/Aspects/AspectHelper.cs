/*******************************************************************************************************************************
 * AK.Commons.Aspects.AspectHelper
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

using System.Collections.Generic;
using AK.Commons.Aspects.Generators;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

#endregion

namespace AK.Commons.Aspects
{
    /// <summary>
    /// Lets you wrap existing classes with AOP, or register MEF-exported classes with AOP.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public static class AspectHelper
    {
        #region Events

        /// <summary>
        /// Fired when the aspect-wrapped code is generated. This can be used to inspect
        /// the generated code in C#, or to modify the generated CodeCompileUnit.
        /// </summary>
        public static event EventHandler<CodeGeneratedEventArgs> CodeGenerated;

        /// <summary>
        /// Fired when the compilation process for the aspect-wrapped generated code is
        /// complete (success or failure). You can use this to inspect the compilation results.
        /// </summary>
        public static event EventHandler<CodeCompiledEventArgs> CodeCompiled;

        #endregion

        #region Methods (Public) - Wrap

        /// <summary>
        /// Wraps the given implementation of a contract with aspects.
        /// </summary>
        /// <typeparam name="TContract">Contract (must be an interface).</typeparam>
        /// <param name="implementation">Implementation object decorated with aspects to wrap (cannot be null).</param>
        /// <returns>
        /// Wrapped instance with generated aspect code based on aspect decorations in the original implementation.
        /// </returns>
        public static TContract Wrap<TContract>(TContract implementation) where TContract : class
        {
            if (implementation == null) throw new ArgumentNullException("implementation");

            var contractType = typeof (TContract);
            if (!contractType.IsInterface) throw new Exception("Contract must be an interface.");

            var implementationType = implementation.GetType();

            var generatedType = GenerateImplementationViaCache(contractType, implementationType);

            var constructor = generatedType.GetConstructor(contractType.AsArray());
            Debug.Assert(constructor != null);

            return (TContract) constructor.Invoke(((object) implementation).AsArray());
        }

        /// <summary>
        /// Lazy-wraps the given implementation of a contract with aspects.
        /// </summary>
        /// <typeparam name="TContract">Contract (must be an interface).</typeparam>
        /// <param name="implementationAccessor">
        /// Function that will yield the implementation object decorated with aspects to wrap.
        /// </param>
        /// <returns>
        /// A lazy-wrapper that, when evaluated, will yield the wrapped instance with generated aspect code
        /// based on aspect decorations in the original implementation.
        /// </returns>
        public static Lazy<TContract> Wrap<TContract>(Func<TContract> implementationAccessor) where TContract : class
        {
            return new Lazy<TContract>(() => Wrap(implementationAccessor()));
        }

        /// <summary>
        /// Lazy-wraps the given implementation of a contract with aspects.
        /// </summary>
        /// <typeparam name="TContract">Contract (must be an interface).</typeparam>
        /// <param name="implementationAccessor">
        /// A lazy-wrapper that will yield the implementation object decorated with aspects to wrap when evaluated.
        /// </param>
        /// <returns>
        /// A lazy-wrapper that, when evaluated, will yield the wrapped instance with generated aspect code
        /// based on aspect decorations in the original implementation.
        /// </returns>
        public static Lazy<TContract> Wrap<TContract>(Lazy<TContract> implementationAccessor) where TContract : class
        {
            return new Lazy<TContract>(() => Wrap(implementationAccessor.Value));
        }

        #endregion

        #region Methods (Public) - WrapMany

        /// <summary>
        /// Wraps the given implementations of a contract with aspects.
        /// </summary>
        /// <typeparam name="TContract">Contract (must be an interface).</typeparam>
        /// <param name="implementations">List of implementation objects decorated with aspects to wrap (cannot be null).</param>
        /// <returns>
        /// List of wrapped instances with generated aspect code based on aspect decorations in the original implementation.
        /// </returns>
        public static IEnumerable<TContract> WrapMany<TContract>(IEnumerable<TContract> implementations)
            where TContract : class
        {
            if (implementations == null) throw new ArgumentNullException("implementations");

            return implementations.Where(x => x != null).Select(Wrap);
        }

        /// <summary>
        /// Lazy-wraps the given implementations of a contract with aspects.
        /// </summary>
        /// <typeparam name="TContract">Contract (must be an interface).</typeparam>
        /// <param name="implementationAccessors">
        /// List of functions that will yield the implementation object decorated with aspects to wrap.
        /// </param>
        /// <returns>
        /// List of lazy-wrappers that, when evaluated, will yield the wrapped instance with generated aspect code
        /// based on aspect decorations in the original implementation.
        /// </returns>
        public static IEnumerable<Lazy<TContract>> WrapMany<TContract>(
            IEnumerable<Func<TContract>> implementationAccessors) where TContract : class
        {
            if (implementationAccessors == null) throw new ArgumentNullException("implementationAccessors");

            return implementationAccessors.Where(x => x != null).Select(Wrap);
        }

        /// <summary>
        /// Lazy-wraps the given implementations of a contract with aspects.
        /// </summary>
        /// <typeparam name="TContract">Contract (must be an interface).</typeparam>
        /// <param name="implementationAccessors">
        /// List of lazy-wrappers that will yield the implementation object decorated with aspects to wrap when evaluated.
        /// </param>
        /// <returns>
        /// List of lazy-wrappers that, when evaluated, will yield the wrapped instance with generated aspect code
        /// based on aspect decorations in the original implementation.
        /// </returns>
        public static IEnumerable<Lazy<TContract>> WrapMany<TContract>(
            IEnumerable<Lazy<TContract>> implementationAccessors) where TContract : class
        {
            if (implementationAccessors == null) throw new ArgumentNullException("implementationAccessors");

            return implementationAccessors.Where(x => x != null).Select(Wrap);
        }

        #endregion

        #region Methods (Public) - MEF Registration

        /// <summary>
        /// Scans the given assemblies, and creates aspect-wrapped exports for all exported types, then adds them
        /// to the MEF container.
        /// </summary>
        /// <param name="container">MEF container. Must be built using an AggregateCatalog.</param>
        /// <param name="assemblies">List of assemblies to scan.</param>
        public static void RegisterForComposition(CompositionContainer container, params Assembly[] assemblies)
        {
            var catalog = container.Catalog as AggregateCatalog;
            if (catalog == null) throw new Exception("Container must have AggregateCatalog to use this method.");

            foreach (var assembly in assemblies)
                RegisterAssemblyForComposition(catalog, assembly);
        }

        #endregion

        #region Methods (Private) - MEF Registration

        private static void RegisterAssemblyForComposition(AggregateCatalog catalog, Assembly assembly)
        {
            var allTypes = assembly.GetTypes();
            var exportedTypes = allTypes.Where(x => x.GetCustomAttributes().Any(y => y is ExportAttribute));

            var generatedTypes =
                (from exportedType in exportedTypes
                 let exportAttribute = exportedType.GetCustomAttribute<ExportAttribute>()
                 where exportAttribute.ContractType != null && exportAttribute.ContractType.IsInterface
                 let contractType = exportAttribute.ContractType
                 let implementationType = exportedType
                 select GenerateImplementationViaCache(contractType, implementationType));

            catalog.Catalogs.Add(new TypeCatalog(generatedTypes));
        }

        #endregion

        #region Methods (Private) - Main Generation

        private static Type GenerateImplementationViaCache(Type contractType, Type implementationType)
        {
            return GeneratedTypeCache.Get(
                contractType,
                implementationType,
                () => GenerateImplementation(contractType, implementationType));
        }

        private static Type GenerateImplementation(Type contractType, Type implementationType)
        {
            var aspectedTypeGenerator = new AspectedTypeGenerator(contractType, implementationType);
            var codeCompileUnit = aspectedTypeGenerator.Generate();
            var generatedCode = GetGeneratedCode(codeCompileUnit);

            if (CodeGenerated != null)
            {
                var eventArgs = new CodeGeneratedEventArgs(
                    contractType, implementationType, generatedCode, codeCompileUnit);

                CodeGenerated(typeof (AspectGenerator), eventArgs);
            }

            var compiledAssembly = CompileUnit(codeCompileUnit, contractType, implementationType);

            var generatedType = compiledAssembly.GetTypes().Single(x => x.GetInterfaces().Contains(contractType));

            return generatedType;
        }

        #endregion

        #region Methods (Private) - Code Generation/Compilation

        private static string GetGeneratedCode(CodeCompileUnit unit)
        {
            string generatedCode;

            using (var stringWriter = new StringWriter())
            using (var codeProvider = new CSharpCodeProvider())
            {
                codeProvider.GenerateCodeFromCompileUnit(
                    unit, stringWriter,
                    new CodeGeneratorOptions
                    {
                        BlankLinesBetweenMembers = true,
                        BracingStyle = "C",
                        IndentString = "    ",
                        VerbatimOrder = true,
                    });

                generatedCode = stringWriter.ToString();
            }

            return generatedCode;
        }

        private static Assembly CompileUnit(CodeCompileUnit unit, Type contractType, Type implementationType)
        {
            using (var codeProvider = new CSharpCodeProvider())
            {
                var results = codeProvider.CompileAssemblyFromDom(
                    new CompilerParameters {GenerateInMemory = true}, unit);

                if (CodeCompiled != null)
                {
                    var eventArgs = new CodeCompiledEventArgs(contractType, implementationType, results);
                    CodeCompiled(typeof (AspectHelper), eventArgs);
                }

                var hasErrors = (from CompilerError error in results.Errors
                                 where !error.IsWarning
                                 select error).Any();

                if (hasErrors) throw new CodeCompilationException(results);

                return results.CompiledAssembly;
            }
        }

        #endregion
    }
}