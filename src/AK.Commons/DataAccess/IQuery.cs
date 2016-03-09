using AK.Commons.Composition;

namespace AK.Commons.DataAccess
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
        IUnitOfWork UnitOfWork { get; set; }
    }
}