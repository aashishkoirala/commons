namespace AK.Commons.Messaging
{
    public interface IMessageHandler<in T>
    {
        void Handle(T message);
    }
}