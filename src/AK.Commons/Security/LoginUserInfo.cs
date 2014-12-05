using System;
using System.Runtime.Serialization;

namespace AK.Commons.Security
{
    [Serializable]
    [DataContract]
    public class LoginUserInfo
    {
        [DataMember]
        public bool UserExists { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string DisplayName { get; set; }
    }
}