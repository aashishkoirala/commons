using System.Collections.Generic;
using System.Linq;

namespace AK.Commons.Commands
{
    public class CommandState
    {
        public bool Success => !this.Errors.Any();
        public string UnitName { get; set; }
        public string NextUnitName { get; set; }
        public CommandUnitState UnitState { get; set; }
        public bool RecoveringFromFailure { get; set; }
        public ICollection<string> Errors { get; } = new List<string>();
        public IDictionary<string, string> Values { get; } = new Dictionary<string, string>();
    }
}