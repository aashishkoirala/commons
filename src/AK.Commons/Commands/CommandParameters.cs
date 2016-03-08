﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace AK.Commons.Commands
{
    public class CommandParameters
    {
        public static readonly CommandParameters Empty = new CommandParameters(null);

        public CommandParameters(object parameters = null)
        {
            this.Values = ConvertObjectToDictionary(parameters);
        }

        public IDictionary<string, string> Values { get; private set; }

        private static IDictionary<string, string> ConvertObjectToDictionary(object obj)
        {
            var result = new Dictionary<string, string>();
            if (obj == null) return result;

            var properties = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanRead);

            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                if (value == null)
                {
                    result[property.Name] = null;
                    continue;
                }

                var type = value.GetType();
                if (type == typeof (string) || type == typeof (DateTime) || type == typeof (TimeSpan) || type.IsPrimitive)
                {
                    result[property.Name] = value.ToString();
                    continue;
                }

                var childResult = ConvertObjectToDictionary(value);
                foreach (var pair in childResult) result[property.Name + "." + pair.Key] = pair.Value;
            }

            return result;
        }

        public void Deserialize(byte[] serialized)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(serialized))
            {
                this.Values = (IDictionary<string, string>) formatter.Deserialize(stream);
            }
        }

        public byte[] Serialize()
        {
            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, this.Values);
                return stream.ToArray();
            }
        }
    }
}