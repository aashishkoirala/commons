using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace AK.Commons.Commands
{
    public class CommandState
    {
        public CommandState(string id)
        {
            this.Id = id;
        }

        public CommandState(byte[] serialized)
        {
            this.Deserialize(serialized);
        }

        public bool Success => !this.Errors.Any();
        public string Id { get; private set; }
        public string UnitName { get; set; }
        public string NextUnitName { get; set; }
        public CommandUnitState UnitState { get; set; }
        public bool RecoveringFromFailure { get; set; }
        public int FailedAttempts { get; set; }
        public ICollection<string> Errors { get; } = new List<string>();
        public IDictionary<string, string> Values { get; } = new Dictionary<string, string>();

        private void Deserialize(byte[] serialized)
        {
            var formatter = new BinaryFormatter();

            IDictionary<string, string> hash;
            using (var stream = new MemoryStream(serialized))
            {
                hash = (IDictionary<string, string>) formatter.Deserialize(stream);
            }

            this.Id = hash[nameof(this.Id)];
            this.UnitName = hash[nameof(this.UnitName)];
            this.NextUnitName = hash[nameof(this.NextUnitName)];
            this.UnitState = (CommandUnitState) Enum.Parse(typeof (CommandUnitState), hash[nameof(this.UnitState)]);
            this.RecoveringFromFailure = bool.Parse(hash[nameof(this.RecoveringFromFailure)]);
            this.FailedAttempts = int.Parse(hash[nameof(this.FailedAttempts)]);

            foreach (var key in hash.Keys.Where(x => x.StartsWith("Error."))) this.Errors.Add(hash[key]);
            foreach (var key in hash.Keys.Where(x => x.StartsWith("Values."))) this.Values[key.Substring(7)] = hash[key];
        }

        public byte[] Serialize()
        {
            var hash = new Dictionary<string, string>
            {
                [nameof(this.Id)] = this.Id,
                [nameof(this.UnitName)] = this.UnitName,
                [nameof(this.NextUnitName)] = this.NextUnitName,
                [nameof(this.UnitState)] = this.UnitState.ToString(),
                [nameof(this.RecoveringFromFailure)] = this.RecoveringFromFailure.ToString(),
                [nameof(this.FailedAttempts)] = this.FailedAttempts.ToString()
            };

            for (var i = 0; i < this.Errors.Count; i++) hash[$"Error.{i}"] = this.Errors.ElementAt(i);
            foreach (var pair in this.Values) hash[$"Values.{pair.Key}"] = pair.Value;

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, hash);
                return stream.ToArray();
            }
        }
    }
}