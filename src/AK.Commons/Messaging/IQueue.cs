using System;

namespace AK.Commons.Messaging
{
    public interface IQueue
    {
        Action<IMessage> MessageReceived { get; set; }
        void Start();
        void Stop();
        void Send(IMessage message);
    }
}