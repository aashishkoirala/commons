/*******************************************************************************************************************************
 * AK.Commons.Web.Composition.ComposableDependencyResolver
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

#endregion

namespace AK.Commons.Web.Composition
{
    /// <summary>
    /// Implementation of Web API IDependencyResolver that uses AppEnvironment.Composer to resolve
    /// dependencies.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class ComposableDependencyResolver : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose() { }

        public object GetService(Type serviceType)
        {
            var exports = AppEnvironment.Composer.ResolveManyLazy(serviceType);
            var export = exports.SingleOrDefault();
            return export != null ? export.Value : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return AppEnvironment.Composer.ResolveMany(serviceType);
        }
    }
}