using System.Collections.Generic;
using System.Runtime.Serialization;

namespace dotnetCampus.WeChatWork.Robots.Models
{
    [DataContract]
    public class TextMessageModel : RequestMessageModel
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "mentioned_list")]
        public IList<string> MentionedList { get; set; }

        [DataMember(Name = "mentioned_mobile_list")]
        public IList<string> MentionedMobileList { get; set; }
    }
}
