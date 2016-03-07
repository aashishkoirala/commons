using System;
using AK.Commons.Composition;

namespace AK.Commons.Commands
{
    public class CommandDefinition
    {
        public CommandDefinition(string name, string[] unitNames, Func<IComposer, string> currentUnitNameFactory = null)
        {
            this.Name = name;
            this.UnitNames = unitNames;
            this.CurrentUnitNameFactory = currentUnitNameFactory;
        }

        public CommandDefinition(byte[] serialized)
        {

        }

        public string Name { get; }
        public string[] UnitNames { get; }
        public Func<IComposer, string> CurrentUnitNameFactory { get; }

        public byte[] Serialize()
        {
            return null;
        }
    }
}