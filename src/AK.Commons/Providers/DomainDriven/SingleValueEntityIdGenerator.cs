using AK.Commons.DomainDriven;

namespace AK.Commons.Providers.DomainDriven
{
    public class SingleValueEntityIdGenerator<T> : IEntityIdGenerator<T> where T : struct
    {
        private readonly T value;

        public SingleValueEntityIdGenerator(T value)
        {
            this.value = value;
        }

        public T Next<TEntity>()
        {
            return this.value;
        }
    }
}