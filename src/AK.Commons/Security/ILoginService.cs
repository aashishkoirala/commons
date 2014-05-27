/*******************************************************************************************************************************
 * AK.Commons.Security.ILoginService
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

using System.Net.Security;
using System.ServiceModel;

#endregion

namespace AK.Commons.Security
{
    /// <summary>
    /// WCF service contract to be implemented by relying parties to provide information about them to an STS.
    /// </summary>
    /// <author>Aashish Koirala</author>
    [ServiceContract(ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface ILoginService
    {
        /// <summary>
        /// Gets information needed to display a login splash page for the relying party.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        LoginSplashInfo GetLoginSplashInfo();

        /// <summary>
        /// Gets information about the requested user.
        /// </summary>
        /// <param name="userName">User name as provided by the identity provider.</param>
        /// <returns>User information.</returns>
        [OperationContract]
        LoginUserInfo GetUser(string userName);

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="userInfo">User information.</param>
        /// <returns>User information on the newly created user.</returns>
        [OperationContract]
        LoginUserInfo CreateUser(LoginUserInfo userInfo);
    }
}