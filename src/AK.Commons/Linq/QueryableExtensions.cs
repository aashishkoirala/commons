using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AK.Commons.Linq
{
    public static class QueryableExtensions
    {
        public static IQueryable<TElement> WhereIn<TElement, TProperty>(this IQueryable<TElement> queryable,
            Expression<Func<TElement, TProperty>> property, TProperty[] values)
        {
            var expr = ConvertArrayToOrList(property, values);
            return queryable.Where(expr);
        }

        public static IQueryable<T> Filter<T, P>(this IQueryable<T> query, P[] idList, Expression<Func<T, P>> member)
        {
            if (idList == null || !idList.Any()) return query;
            var id = idList[0];
            Expression<Func<T, bool>> predicate;
            Expression body;
            if (idList.Length == 1)
            {
                body = Expression.Equal(member.Body, Expression.Constant(id));
            }
            else
            {
                body = Expression.Call(null, typeof(Enumerable).GetMethods().Single(x => x.Name == "Contains" && x.GetParameters().Length == 2).MakeGenericMethod(typeof(P)), Expression.Constant(idList), member.Body);
            }
            predicate = (Expression<Func<T, bool>>)Expression.Lambda(body, member.Parameters);

            return query.Where(predicate);
        }

        private static Expression<Func<TElement, bool>> ConvertArrayToOrList<TElement, TProperty>(
            Expression<Func<TElement, TProperty>> property, IEnumerable<TProperty> values)
        {
            Expression convertedExpression = null;
            foreach (var id in values)
            {
                var eachExpression = Expression.Equal(property.Body, Expression.Constant(id));
                convertedExpression = convertedExpression == null ? eachExpression : Expression.OrElse(convertedExpression, eachExpression);
            }
            if (convertedExpression == null) convertedExpression = Expression.Constant(false);
            return (Expression<Func<TElement, bool>>) Expression.Lambda(convertedExpression, property.Parameters.Single());
        }
    }
}