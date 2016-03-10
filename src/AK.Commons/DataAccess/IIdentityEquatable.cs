using System;
using System.Linq.Expressions;

namespace AK.Commons.DataAccess
{
    public interface IIdentityEquatable<T>
    {
        Func<T, Expression<Func<T, bool>>> IdentityEqualizer { get; }
    }
}