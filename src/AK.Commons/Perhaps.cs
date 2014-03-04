/*******************************************************************************************************************************
 * AK.Commons.Perhaps
 * Copyright © 2013 Aashish Koirala <http://aashishkoirala.github.io>
 * 
 * This file is part of Aashish Koirala's Commons Library (AKCL).
 *  
 * AKCL is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AKCL is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AKCL.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * The code in this particular file inspired by Brad Wilson's Maybe<T> (https://gist.github.com/bradwilson/9200743) and
 * JaredPar's Option<T> (http://blogs.msdn.com/b/jaredpar/archive/2008/10/08/functional-c-providing-an-option-part-2.aspx)
 * 
 *******************************************************************************************************************************/

#region Namespace Imports

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace AK.Commons
{
    #region Perhaps

    /// <summary>
    /// Represents a wrapper around a value that may or may not be there depending on what operation gave you this instance.
    /// Casting operators are defiend so that you can convert between the wrapped and underlying values implicitly.
    /// </summary>
    /// <typeparam name="T">Type of the underlying value.</typeparam>
    /// <author>Aashish Koirala</author>
    /// <inspiredby>
    /// Brad Wilson's "Maybe" - https://gist.github.com/bradwilson/9200743
    /// </inspiredby>
    /// <inspiredby>
    /// JaredPar's "Option" - http://blogs.msdn.com/b/jaredpar/archive/2008/10/08/functional-c-providing-an-option-part-2.aspx
    /// </inspiredby>
    public struct Perhaps<T> : IEquatable<Perhaps<T>>
    {
        #region Internals

        private static readonly Perhaps<T> notThere = new Perhaps<T>(default(T), false);

        private readonly T value;
        private readonly bool isThere;
        private readonly Exception exception;

        private Perhaps(T value, bool isThere, Exception exception = null)
        {
            this.value = value;
            this.isThere = isThere;
            this.exception = exception;
        }

        #endregion

        #region Public Constructor + FromException

        /// <summary>
        /// Wraps the given value in a "Perhaps".
        /// </summary>
        /// <param name="value">Underlying value.</param>
        public Perhaps(T value) : this(value, true) { }

        /// <summary>
        /// Creates an errored "Perhaps" from the given exception.
        /// </summary>
        /// <param name="exception">Exception object.</param>
        /// <returns>Perhaps object with HasError set to True and Exception set to the given value.</returns>
        public static Perhaps<T> FromException(Exception exception)
        {
            return new Perhaps<T>(default(T), false, exception);
        }

        #endregion

        #region Public Properties & Methods

        /// <summary>
        /// A Perhaps instance that represents the non-existence of the underlying value.
        /// </summary>
        public static Perhaps<T> NotThere
        {
            get { return notThere; }
        }

        /// <summary>
        /// Gets the underlying value, if it's there. Throws if not.
        /// </summary>
        public T Value
        {
            get
            {
                if (!this.isThere) throw new InvalidOperationException("No value assigned.");
                return this.value;
            }
        }

        /// <summary>
        /// Gets the underlying value, if it's there. Returns the type default if not.
        /// </summary>
        public T ValueOrDefault
        {
            get { return !this.isThere ? default(T) : this.value; }
        }

        /// <summary>
        /// Is the underlying value there?
        /// </summary>
        public bool IsThere
        {
            get { return this.isThere; }
        }

        /// <summary>
        /// Is this instance errored?
        /// </summary>
        public bool HasError
        {
            get { return this.exception != null; }
        }

        /// <summary>
        /// Exception, if this instance is errored.
        /// </summary>
        public Exception Exception
        {
            get { return this.exception; }
        }

        /// <summary>
        /// Does the thing passed in if the underlying value exists.
        /// </summary>
        /// <param name="action">Thing to do with underlying value.</param>
        public void DoIfThere(Action<T> action)
        {
            if (this.isThere) action(this.value);
        }

        /// <summary>
        /// Does the thing passed in if the underlying value exists.
        /// </summary>
        /// <param name="action">Thing to do with underlying value.</param>
        /// <param name="defaultAccessor">Thing to call to get the default value if underlying value is not there.</param>
        /// <returns>The result of the thing done, or the default based on whether the underlying value exists.</returns>
        public TResult DoIfThere<TResult>(Func<T, TResult> action, Func<TResult> defaultAccessor)
        {
            return this.isThere ? action(this.value) : defaultAccessor();
        }

        /// <summary>
        /// Does the thing passed in if the underlying value exists.
        /// </summary>
        /// <param name="action">Thing to do with underlying value.</param>
        /// <param name="defaultValue">The default value if underlying value is not there.</param>
        /// <returns>The result of the thing done, or the default based on whether the underlying value exists.</returns>
        public TResult DoIfThere<TResult>(Func<T, TResult> action, TResult defaultValue)
        {
            return this.DoIfThere(action, () => defaultValue);
        }

        #endregion

        #region Operators (Casting/Equality)

        public static implicit operator Perhaps<T>(T value)
        {
            return new Perhaps<T>(value);
        }

        public static implicit operator T(Perhaps<T> value)
        {
            return value.ValueOrDefault;
        }

        public static bool operator ==(Perhaps<T> left, Perhaps<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Perhaps<T> left, Perhaps<T> right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region IEquatable/Supporting

        public bool Equals(Perhaps<T> other)
        {
            if (!this.isThere) return !other.isThere;
            return EqualityComparer<T>.Default.Equals(this.value, other.value);
        }

        public override bool Equals(object obj)
        {
            if (obj is Perhaps<T>)
            {
                return this.Equals((Perhaps<T>) obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return !this.isThere ? 0 : EqualityComparer<T>.Default.GetHashCode(this.value);
        }

        #endregion

        #region Perhaps.Try Methods

        /// <summary>
        /// Performs the given TryX type operation and returns the results as a Perhaps.
        /// </summary>
        /// <typeparam name="TValue">Type of out parameter returned by the operation.</typeparam>
        /// <param name="operation">The operation to perform.</param>
        /// <returns>
        /// A Perhaps instance that wraps the returned value if TryX returned True,
        /// or "NotThere" if it returned False. An errored Perhaps instance if it threw an exception.
        /// </returns>
        public static Perhaps<TValue> Try<TValue>(PerhapsOperation<TValue> operation)
        {
            try
            {
                TValue value;
                if (!operation(out value)) return Perhaps<TValue>.NotThere;
                return value;
            }
            catch (Exception ex)
            {
                return Perhaps<TValue>.FromException(ex);
            }
        }

        /// <summary>
        /// Performs the given operation and returns a Perhaps representing whether the operation
        /// threw an exception.
        /// </summary>
        /// <param name="action">The operation to perform.</param>
        /// <returns>
        /// An errored Perhaps instance if it threw an exception. An empty one if it didn't.
        /// </returns>
        public static Perhaps<object> Try(Action action)
        {
            try
            {
                action();
                return new Perhaps<object>(null);
            }
            catch (Exception ex)
            {
                return Perhaps<object>.FromException(ex);
            }
        }

        /// <summary>
        /// Performs the given operation and returns the results as a Perhaps.
        /// </summary>
        /// <typeparam name="TValue">Type of the value returned by the operation.</typeparam>
        /// <param name="action">The operation to perform.</param>
        /// <returns>
        /// A Perhaps instance that wraps the returned value, or 
        /// an errored Perhaps instance if it threw an exception.
        /// </returns>
        public static Perhaps<TValue> Try<TValue>(Func<TValue> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                return Perhaps<TValue>.FromException(ex);
            }
        }

        #endregion
    }

    #endregion

    #region PerhapsOperation

    /// <summary>
    /// Represents a call to a "TryX" type operation that returns a boolean and then returns the actual value
    /// as an out parameter. To be used with Perhaps constructs to wrap "TryX" type methods.
    /// </summary>
    /// <typeparam name="T">Type of the underlying value.</typeparam>
    /// <param name="value">Out parameter.</param>
    /// <author>Aashish Koirala</author>
    public delegate bool PerhapsOperation<T>(out T value);

    #endregion

    #region PerhapsRelatedExtensions

    /// <summary>
    /// Extension methods for various types that make use of Perhaps to provide convenience features.
    /// </summary>
    /// <author>Aashish Koirala</author>
    /// <inspiredby>
    /// Brad Wilson's "Maybe" - https://gist.github.com/bradwilson/9200743
    /// </inspiredby>
    public static class PerhapsRelatedExtensions
    {
        #region IDictionary<K, V>

        /// <summary>
        /// Looks for the given key in the given dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of key.</typeparam>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="dictionary">Dictionary to look in.</param>
        /// <param name="key">Value of key.</param>
        /// <returns>
        /// The value wrapped in a Perhaps if found, or "NotThere" if not
        /// found or if the dictionary is null.
        /// </returns>
        public static Perhaps<TValue> LookFor<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null) return Perhaps<TValue>.NotThere;

            TValue value;
            if (!dictionary.TryGetValue(key, out value)) return Perhaps<TValue>.NotThere;

            return value;
        }

        #endregion

        #region String Parsing

        /// <summary>
        /// Tries to parse the given string as an integer.
        /// </summary>
        /// <param name="input">String to parse.</param>
        /// <returns>
        /// Parsed value wrapped as Perhaps if parsed, or "NotThere" if 
        /// input was empty or could not be parsed.
        /// </returns>
        public static Perhaps<int> ParseInteger(this string input)
        {
            return string.IsNullOrWhiteSpace(input)
                       ? Perhaps<int>.NotThere
                       : Perhaps<int>.Try((out int i) => int.TryParse(input, out i));
        }

        /// <summary>
        /// Tries to parse the given string as a long integer.
        /// </summary>
        /// <param name="input">String to parse.</param>
        /// <returns>
        /// Parsed value wrapped as Perhaps if parsed, or "NotThere" if 
        /// input was empty or could not be parsed.
        /// </returns>
        public static Perhaps<long> ParseLong(this string input)
        {
            return string.IsNullOrWhiteSpace(input)
                       ? Perhaps<long>.NotThere
                       : Perhaps<long>.Try((out long l) => long.TryParse(input, out l));
        }

        /// <summary>
        /// Tries to parse the given string a float.
        /// </summary>
        /// <param name="input">String to parse.</param>
        /// <returns>
        /// Parsed value wrapped as Perhaps if parsed, or "NotThere" if 
        /// input was empty or could not be parsed.
        /// </returns>
        public static Perhaps<float> ParseFloat(this string input)
        {
            return string.IsNullOrWhiteSpace(input)
                       ? Perhaps<float>.NotThere
                       : Perhaps<float>.Try((out float f) => float.TryParse(input, out f));
        }

        /// <summary>
        /// Tries to parse the given string as a double.
        /// </summary>
        /// <param name="input">String to parse.</param>
        /// <returns>
        /// Parsed value wrapped as Perhaps if parsed, or "NotThere" if 
        /// input was empty or could not be parsed.
        /// </returns>
        public static Perhaps<double> ParseDouble(this string input)
        {
            return string.IsNullOrWhiteSpace(input)
                       ? Perhaps<double>.NotThere
                       : Perhaps<double>.Try((out double d) => double.TryParse(input, out d));
        }

        /// <summary>
        /// Tries to parse the given string a decimal.
        /// </summary>
        /// <param name="input">String to parse.</param>
        /// <returns>
        /// Parsed value wrapped as Perhaps if parsed, or "NotThere" if 
        /// input was empty or could not be parsed.
        /// </returns>
        public static Perhaps<decimal> ParseDecimal(this string input)
        {
            return string.IsNullOrWhiteSpace(input)
                       ? Perhaps<decimal>.NotThere
                       : Perhaps<decimal>.Try((out decimal d) => decimal.TryParse(input, out d));
        }

        /// <summary>
        /// Tries to parse the given string as a boolean.
        /// </summary>
        /// <param name="input">String to parse.</param>
        /// <returns>
        /// Parsed value wrapped as Perhaps if parsed, or "NotThere" if 
        /// input was empty or could not be parsed.
        /// </returns>
        public static Perhaps<bool> ParseBoolean(this string input)
        {
            return string.IsNullOrWhiteSpace(input)
                       ? Perhaps<bool>.NotThere
                       : Perhaps<bool>.Try((out bool b) => bool.TryParse(input, out b));
        }

        /// <summary>
        /// Tries to parse the given string as a DateTime.
        /// </summary>
        /// <param name="input">String to parse.</param>
        /// <returns>
        /// Parsed value wrapped as Perhaps if parsed, or "NotThere" if 
        /// input was empty or could not be parsed.
        /// </returns>
        public static Perhaps<DateTime> ParseDateTime(this string input)
        {
            return string.IsNullOrWhiteSpace(input)
                       ? Perhaps<DateTime>.NotThere
                       : Perhaps<DateTime>.Try((out DateTime d) => DateTime.TryParse(input, out d));
        }

        #endregion

        #region IEnumerable<T>

        // In these methods, we're using the "First", "Last" and "Single" methods within a try/catch
        // rather than the "FirstOrDefault", "SingleOrDefault" etc. methods because we need to
        // know whether a value was not found versus a value was found but happens to be NULL or
        // the type-default.

        /// <summary>
        /// Perhaps-ifies the the LINQ First method.
        /// </summary>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="values">List to operate on.</param>
        /// <param name="predicate">(Optional) Predicate representing values to match.</param>
        /// <returns>
        /// The first occurrence, if any.
        /// If not found, or if the list is null, returns "NotThere".
        /// </returns>
        public static Perhaps<TValue> PerhapsFirst<TValue>(
            this IEnumerable<TValue> values, Func<TValue, bool> predicate = null)
        {
            if (values == null) return Perhaps<TValue>.NotThere;
            try
            {
                return predicate == null ? values.First() : values.First(predicate);
            }
            catch (InvalidOperationException)
            {
                return Perhaps<TValue>.NotThere;
            }
        }

        /// <summary>
        /// Perhaps-ifies the the LINQ Single method.
        /// </summary>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="values">List to operate on.</param>
        /// <param name="predicate">(Optional) Predicate representing values to match.</param>
        /// <returns>
        /// The one and only occurrence, if any.
        /// If not found, or if the list is null, returns "NotThere".
        /// </returns>
        public static Perhaps<TValue> PerhapsSingle<TValue>(
            this IEnumerable<TValue> values, Func<TValue, bool> predicate = null)
        {
            if (values == null) return Perhaps<TValue>.NotThere;
            try
            {
                return predicate == null ? values.Single() : values.Single(predicate);
            }
            catch (InvalidOperationException)
            {
                return Perhaps<TValue>.NotThere;
            }
        }

        /// <summary>
        /// Perhaps-ifies the the LINQ First method.
        /// </summary>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="values">List to operate on.</param>
        /// <param name="predicate">(Optional) Predicate representing values to match.</param>
        /// <returns>
        /// The last occurrence, if any.
        /// If not found, or if the list is null, returns "NotThere".
        /// </returns>
        public static Perhaps<TValue> PerhapsLast<TValue>(
            this IEnumerable<TValue> values, Func<TValue, bool> predicate = null)
        {
            if (values == null) return Perhaps<TValue>.NotThere;
            try
            {
                return predicate == null ? values.Last() : values.Last(predicate);
            }
            catch (InvalidOperationException)
            {
                return Perhaps<TValue>.NotThere;
            }
        }

        #endregion
    }

    #endregion
}