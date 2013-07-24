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
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace AK.Commons.Services.Hosting
{
    public class CompositionInstanceProvider : IInstanceProvider
    {
        private readonly Type serviceContractType;

        public CompositionInstanceProvider(Type serviceContractType)
        {
            this.serviceContractType = serviceContractType;
        }

        public object GetInstance(InstanceContext instanceContext, System.ServiceModel.Channels.Message message)
        {
            var importDefinition = new ImportDefinition(
                x => x.ContractName.Equals(this.serviceContractType.FullName),
                this.serviceContractType.FullName, ImportCardinality.ZeroOrMore,
                false, false);
            var atomicComposition = new AtomicComposition();

            IEnumerable<Export> extensions;
            if (!AppEnvironment.Composer.Container.TryGetExports(importDefinition, atomicComposition, out extensions))
                return null;

            var extension = extensions != null ? extensions.FirstOrDefault() : null;
            return extension != null ? extension.Value : null;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}