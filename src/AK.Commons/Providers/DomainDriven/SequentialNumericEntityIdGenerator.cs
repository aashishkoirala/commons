using System;
using AK.Commons.DomainDriven;

namespace AK.Commons.Providers.DomainDriven
{
    public class SequentialNumericEntityIdGenerator<T> : IEntityIdGenerator<T> where T : struct
    {
        private readonly long step;
        private long current;

        public SequentialNumericEntityIdGenerator(long start = 1, long step = 1)
        {
            this.current = start;
            this.step = step;
        }

        public T Next<TEntity>()
        {
            var next = (T) Convert.ChangeType(this.current, typeof (T));
            this.current += this.step;

            return next;
        }
    }
}