using System.Collections.Generic;
using AK.Commons.DomainDriven;

namespace AK.Commons.Providers.DomainDriven
{
    public class ListEntityIdGenerator<T> : IEntityIdGenerator<T> where T : struct
    {
        private readonly IEnumerator<T> enumerator;

        public ListEntityIdGenerator(IEnumerable<T> list)
        {
            this.enumerator = list.GetEnumerator();
        }

        public T Next<TEntity>() where TEntity : IEntity<TEntity, T>
        {
            this.enumerator.MoveNext();
            return this.enumerator.Current;
        }
    }
}