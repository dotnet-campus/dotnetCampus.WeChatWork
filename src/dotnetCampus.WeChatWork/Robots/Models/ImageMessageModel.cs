using System.Runtime.Serialization;

namespace dotnetCampus.WeChatWork.Robots.Models
{
    [DataContract]
    public class ImageMessageModel : RequestMessageModel
    {
        [DataMember(Name = "base64")]
        public string Base64 { get; set; }

        [DataMember(Name = "md5")]
        public string Md5 { get; set; }
    }
}
