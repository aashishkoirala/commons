using System;

namespace AK.Commons.Composition
{
    public sealed class Void : IEquatable<Void>, IComparable<Void>, IComparable
    {
        public static readonly Void Value = new Void();

        public bool Equals(Void other) => true;
        public override bool Equals(object obj) => obj is Void;
        public int CompareTo(Void other) => 0;
        public int CompareTo(object obj) => obj is Void ? 0 : 1;
        public override int GetHashCode() => 0;
        public static bool operator ==(Void first, Void second) => true;
        public static bool operator !=(Void first, Void second) => false;
        public override string ToString() => string.Empty;
    }
}