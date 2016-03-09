namespace AK.Commons.Composition
{
    public interface IRequest<out TResult>
    {        
    }

    public interface IRequest : IRequest<Void>
    {        
    }
}