/*******************************************************************************************************************************
 * AK.Commons.DomainDriven.DomainObjects
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

using System;

namespace AK.Commons.DomainDriven
{
    /// <summary>
    /// Represents an entity in domain-driven design.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <typeparam name="TKey">Type of entity key.</typeparam>
    public interface IEntity<TEntity, out TKey> : IEquatable<TEntity>
    {
        /// <summary>
        /// Gets the Id of the entity.
        /// </summary>
        TKey Id { get; }
    }

    /// <summary>
    /// Represents a value-object in domain-driven design.
    /// </summary>
    /// <typeparam name="TValueObject">Type of value-object.</typeparam>
    public interface IValueObject<TValueObject> : IEquatable<TValueObject>
    {
    }

    /// <summary>
    /// Represents an aggregate root in domain-driven design.
    /// </summary>
    /// <typeparam name="TAggregateRoot">Type of aggregate root.</typeparam>
    /// <typeparam name="TKey">Type of aggregate root key.</typeparam>
    public interface IAggregateRoot<TAggregateRoot, out TKey> : IEntity<TAggregateRoot, TKey>
    {
    }
}