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