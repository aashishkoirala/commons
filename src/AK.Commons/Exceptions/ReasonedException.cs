/*******************************************************************************************************************************
 * AK.Commons.Exceptions.ReasonedException
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
 *******************************************************************************************************************************/

#region Namespace Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

#endregion

namespace AK.Commons.Exceptions
{
    /// <summary>
    /// This base class provides common functionality for exceptions with enumerated "reasons" and should be
    /// inherited in order to create new types of such reasoned exceptions.
    /// </summary>
    /// <typeparam name="TReasonEnum">An enumeration to use to define reasons for the exception.</typeparam>
    [Serializable]
    public abstract class ReasonedException<TReasonEnum> : ApplicationException, ISerializableException 
        where TReasonEnum : struct
    {
        #region Fields

        private readonly DateTime timeStamp = DateTime.Now;
        private readonly TReasonEnum reason;
        private IList<string> causeDescriptions;
        private string rootCauseDescription;
        private string serializedData;

        #endregion

        #region Constructors

        protected ReasonedException(TReasonEnum reason) : base(string.Empty)
        {
            this.reason = reason;
        }

        protected ReasonedException(TReasonEnum reason, string message) : base(message)
        {
            this.reason = reason;
        }

        protected ReasonedException(TReasonEnum reason, Exception innerException) : base(string.Empty, innerException)
        {
            this.reason = reason;
        }

        protected ReasonedException(TReasonEnum reason, string message, Exception innerException) : base(message, innerException)
        {            
            this.reason = reason;
        }

        protected ReasonedException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        #endregion

        #region Properties

        /// <summary>
        /// Gets the timestamp instance of the exception.
        /// </summary>
        public DateTime TimeStamp { get { return this.timeStamp; } }

        /// <summary>
        /// Gets the message for the exception.
        /// </summary>
        public override string Message
        {
            get
            {
                var reasonMessage = this.GetReasonDescription(this.reason);
                var additionalMessage = base.Message;

                return GetMessageForReason(reasonMessage, additionalMessage);
            }
        }

        /// <summary>
        /// Gets the reason for the exception.
        /// </summary>
        public TReasonEnum Reason
        {
            get { return this.reason; }
        }

        /// <summary>
        /// Gets a list of descriptions for underlying causes of the exception
        /// (derived from the InnerExceptions tree).
        /// </summary>
        public IEnumerable<string> CauseDescriptions
        {
            get
            {
                this.AssignCauses();
                return this.causeDescriptions;
            }
        }

        /// <summary>
        /// Gets the description of the root cause, if any. The root cause is
        /// the innermost exception in the InnerExceptions tree.
        /// </summary>
        public string RootCauseDescription
        {
            get
            {
                this.AssignCauses();
                return this.rootCauseDescription;
            }
        }

        /// <summary>
        /// Gets the serialized data.
        /// </summary>
        public string SerializedData
        {
            get
            {
                this.Serialize();
                return this.serializedData;
            }
        }

        #endregion

        #region Methods (Protected Abstract/Virtual)

        /// <summary>
        /// This method must be overridden by child classes to map reason codes
        /// to their human-readable descriptions.
        /// </summary>
        /// <param name="reason">Reason code.</param>
        /// <returns>Human-readable description.</returns>
        protected abstract string GetReasonDescription(TReasonEnum reason);

        /// <summary>
        /// Override this method to provide any extra information during serialization.
        /// </summary>
        /// <param name="serializationData">Serialization data dictionary to add to.</param>
        protected virtual void AddSerializationData(IDictionary<string, string> serializationData) { }

        #endregion

        #region Methods (Private Helper)

        private static string GetMessageForReason(string reasonMessage, string additionalMessage)
        {
            return string.IsNullOrWhiteSpace(additionalMessage) ? 
                reasonMessage : string.Format("{0} {1}", reasonMessage, additionalMessage);
        }

        private void AssignCauses()
        {
            if (this.causeDescriptions != null && this.rootCauseDescription != null) return;

            var causeList = new List<Exception>();
            var exception = this.InnerException;
            while (exception != null)
            {
                causeList.Add(exception);
                exception = exception.InnerException;
            }
            var rootCause = causeList.LastOrDefault();

            if (rootCause != null) causeList.Remove(rootCause);

            this.causeDescriptions = causeList.Select(x => x.ToString()).ToList();
            this.rootCauseDescription = rootCause != null ? rootCause.ToString() : string.Empty;
        }

        private void Serialize()
        {
            if (this.serializedData != null) return;

            var serializedDataHash = new Dictionary<string, string>
            {
                {"TimeStamp", this.TimeStamp.ToString()},
                {"Details", this.ToString()},
                {"Reason", this.Reason.ToString()},
                {"RootCause", this.RootCauseDescription}
            };

            this.causeDescriptions.For((s, i) => serializedDataHash["Cause" + i.ToString()] = s);

            this.AddSerializationData(serializedDataHash);

            var xmlDocument = new XmlDocument();
            var xmlRoot = xmlDocument.CreateElement("Exception");
            serializedDataHash.ForEach(pair =>
            {
                var xmlElement = xmlDocument.CreateElement(pair.Key);
                xmlElement.InnerText = pair.Value;
                xmlRoot.AppendChild(xmlElement);
            });

            this.serializedData = xmlRoot.OuterXml;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SerializedData", this.SerializedData);
        }

        #endregion
    }
}