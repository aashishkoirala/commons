using System.Collections.Generic;

namespace AK.Commons.Commands
{
    public class CommandParameters
    {
        public static readonly CommandParameters Empty = new CommandParameters(null);

        public CommandParameters(object parameters)
        {
            this.Values = new Dictionary<string, string>();
            if (parameters == null) return;
        }

        public IDictionary<string, string> Values { get; }
    }
}