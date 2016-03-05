using System.Collections.Generic;

namespace AK.Commons.Messaging
{
    public interface IMessage
    {
        IDictionary<string, object> Headers { get; }
        T Body<T>();
    }
}