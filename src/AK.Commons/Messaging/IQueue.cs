using System;

namespace AK.Commons.Messaging
{
    public interface IQueue
    {
        IMessage CreateMessage<T>(T body);
        Action<IMessage> MessageReceived { get; set; }
        void Start();
        void Stop();
        void Send(IMessage message);
    }
}