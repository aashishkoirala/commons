using System;

namespace AK.Commons.Messaging
{
    public interface IHub
    {
        IQueue Subscribe(string subscriptionName, Func<IMessage, bool> condition = null);
        void Unsubscribe(string subscriptionName);
        void Publish(IMessage message);
    }
}