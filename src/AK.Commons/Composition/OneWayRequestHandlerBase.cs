namespace AK.Commons.Composition
{
    public abstract class OneWayRequestHandlerBase<TRequest> : IRequestHandler<TRequest, Void> where TRequest : IRequest
    {
        public Void Handle(TRequest request)
        {
            this.HandleInternal(request);
            return Void.Value;
        }

        protected abstract void HandleInternal(TRequest request);
    }
}