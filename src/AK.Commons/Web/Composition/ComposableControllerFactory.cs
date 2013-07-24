/*******************************************************************************************************************************
 * AK.Commons.Web.Composition.ComposableControllerFactory
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
using System.Web.Mvc;
using System.Web.Routing;

#endregion

namespace AK.Commons.Web.Composition
{
    /// <summary>
    /// Implementation of IControllerFactory that spawns MVC controllers using AppEnvironment.Composer.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public class ComposableControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            var controller = AppEnvironment.Composer.Resolve(controllerType) as IController;

            return controller ?? base.GetControllerInstance(requestContext, controllerType);
        }
    }
}