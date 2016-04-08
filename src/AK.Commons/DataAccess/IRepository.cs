/*******************************************************************************************************************************
 * AK.Commons.DataAccess.IRepository
 * Copyright © 2013-2014 Aashish Koirala <http://aashishkoirala.github.io>
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AK.Commons.DataAccess
{
    /// <summary>
    /// Represents a generic repository.
    /// </summary>
    /// <author>Aashish Koirala</author>
    public interface IRepository
    {
    }

    /// <summary>
    /// Represents a repository for the given type.
    /// </summary>
    /// <typeparam name="T">Type of entity this repository houses.</typeparam>
    /// <author>Aashish Koirala</author>
    public interface IRepository<T> : IRepository where T : class
    {
        T GetOrCreate<TKey>(TKey id, Func<TKey, T> creator = null) where TKey : struct;

        IEnumerable<T> GetOrCreateList<TKey>(IEnumerable<TKey> idList, Func<TKey, T> creator = null) where TKey : struct;

        /// <summary>
        /// A LINQ-queryable interface for data access to the repository.
        /// </summary>
        IQueryable<T> Query { get; }

        /// <summary>
        /// Saves the given entity to the repository.
        /// </summary>
        /// <param name="thing">Entity to save.</param>
        void Save(T thing);

        /// <summary>
        /// Deletes the given entity from the repository.
        /// </summary>
        /// <param name="thing">Entity to delete.</param>
        void Delete(T thing);
    }

    public static class RepositoryMappingExtensions
    {
        public static TOut Map<TIn, TOut, TKey>(this IRepository<TOut> repository,
            TIn inObject,
            Action<ObjectMapper<TIn, TOut>> mapper = null,
            Func<TKey, TOut> creator = null,
            Expression<Func<TIn, TKey>> keyExpression = null)
            where TIn : class
            where TOut : class
            where TKey : struct
        {
            if (keyExpression == null)
            {
                var targetExpression = Expression.Parameter(typeof (TIn), "x");
                var idAccessExpression = Expression.PropertyOrField(targetExpression, "Id");
                keyExpression = (Expression<Func<TIn, TKey>>) Expression.Lambda(idAccessExpression, targetExpression);
            }

            var key = keyExpression.Compile()(inObject);
            var outObject = repository.GetOrCreate(key, creator);
            var objectMapper = new ObjectMapper<TIn, TOut>(inObject, outObject);
            if (mapper == null) objectMapper.AutoMap();
            else mapper(objectMapper);
            return outObject;
        }

        public static void MapAndSave<TIn, TOut, TKey>(this IRepository<TOut> repository,
            TIn inObject,
            Action<ObjectMapper<TIn, TOut>> mapper = null,
            Func<TKey, TOut> creator = null,
            Expression<Func<TIn, TKey>> keyExpression = null)
            where TIn : class
            where TOut : class
            where TKey : struct
        {
            var thing = repository.Map(inObject, mapper, creator, keyExpression);
            repository.Save(thing);
        }

        public static IEnumerable<TOut> MapList<TIn, TOut, TKey>(this IRepository<TOut> repository,
            IEnumerable<TIn> inObjects,
            IEnumerable<TOut> existingOutObjects = null,
            Action<ObjectMapper<TIn, TOut>> mapper = null,
            Func<TKey, TOut> creator = null,
            Expression<Func<TIn, TKey>> inKeyExpression = null,
            Expression<Func<TOut, TKey>> outKeyExpression = null)
            where TIn : class
            where TOut : class
            where TKey : struct
        {
            if (inKeyExpression == null)
            {
                var targetExpression = Expression.Parameter(typeof (TIn), "x");
                var idAccessExpression = Expression.PropertyOrField(targetExpression, "Id");
                inKeyExpression = (Expression<Func<TIn, TKey>>) Expression.Lambda(idAccessExpression, targetExpression);
            }

            if (outKeyExpression == null)
            {
                var targetExpression = Expression.Parameter(typeof (TIn), "x");
                var idAccessExpression = Expression.PropertyOrField(targetExpression, "Id");
                outKeyExpression = (Expression<Func<TOut, TKey>>) Expression.Lambda(idAccessExpression, targetExpression);
            }

            var inKeyFunc = inKeyExpression.Compile();
            var outKeyFunc = outKeyExpression.Compile();
            var keyList = inObjects.Select(inKeyFunc).ToArray();
            var outObjects = repository.GetOrCreateList(keyList, creator);
            outObjects.ForEach(x =>
            {
                var outKey = outKeyFunc(x);
                var inObject = inObjects.Single(y => inKeyFunc(y).Equals(outKey));
                var objectMapper = new ObjectMapper<TIn, TOut>(inObject, x);
                if (mapper == null) objectMapper.AutoMap();
                else mapper(objectMapper);
            });

            existingOutObjects = existingOutObjects ?? new TOut[0];
            if (!existingOutObjects.Any()) return outObjects;

            var objectsToRemove = existingOutObjects.Where(x => !keyList.Contains(outKeyFunc(x))).ToArray();
            foreach (var objectToRemove in objectsToRemove) repository.Delete(objectToRemove);

            return outObjects;
        }

        public static void MapAndSaveList<TIn, TOut, TKey>(this IRepository<TOut> repository,
            IEnumerable<TIn> inObjects,
            IEnumerable<TOut> existingOutObjects = null,
            Action<ObjectMapper<TIn, TOut>> mapper = null,
            Func<TKey, TOut> creator = null,
            Expression<Func<TIn, TKey>> inKeyExpression = null,
            Expression<Func<TOut, TKey>> outKeyExpression = null)
            where TIn : class
            where TOut : class
            where TKey : struct
        {
            var list = repository.MapList(inObjects, existingOutObjects, mapper, creator, inKeyExpression, outKeyExpression);
            list.ForEach(repository.Save);
        }
    }

    public class ObjectMapper<TSource, TTarget> where TSource : class where TTarget : class
    {
        public ObjectMapper(TSource source, TTarget target)
        {
            this.Source = source;
            this.Target = target;
        }

        public TSource Source { get; }
        public TTarget Target { get; }

        public AfterAutoMap<TSource, TTarget> AutoMap()
        {
            var sourceProperties = typeof (TSource)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead);

            foreach (var sourceProperty in sourceProperties)
            {
                var targetProperty = typeof (TTarget)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .SingleOrDefault(x => x.CanWrite && x.Name == sourceProperty.Name && x.PropertyType == sourceProperty.PropertyType);

                if (targetProperty == null) continue;

                var sourceValue = sourceProperty.GetValue(this.Source);
                targetProperty.SetValue(this.Target, sourceValue);
            }

            return new AfterAutoMap<TSource, TTarget>(this.Source, this.Target);
        }
    }

    public class AfterAutoMap<TSource, TTarget> where TSource : class where TTarget : class
    {
        private readonly TSource source;
        private readonly TTarget target;

        public AfterAutoMap(TSource source, TTarget target)
        {
            this.source = source;
            this.target = target;
        }

        public void Then(Action<TSource, TTarget> mapper)
        {
            mapper(this.source, this.target);
        }
    }
}