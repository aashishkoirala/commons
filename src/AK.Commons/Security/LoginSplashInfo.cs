using System.Runtime.Serialization;

namespace AK.Commons.Security
{
    [DataContract]
    public class LoginSplashInfo
    {
        [DataMember]
        public string ApplicationName { get; set; }

        [DataMember]
        public string TitleCss { get; set; }

        [DataMember]
        public string TitleHtml { get; set; }

        [DataMember]
        public string DescriptionCss { get; set; }

        [DataMember]
        public string DescriptionHtml { get; set; }

        [DataMember]
        public string BannerImageUrl { get; set; }
    }
}