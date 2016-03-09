using AK.Commons.Composition;

namespace AK.Commons.DataAccess
{
    public interface ICommand : IRequest
    {
        IUnitOfWork UnitOfWork { get; set; }
    }
}