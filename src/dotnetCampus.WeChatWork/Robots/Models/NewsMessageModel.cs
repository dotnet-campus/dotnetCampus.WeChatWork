using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace dotnetCampus.WeChatWork.Robots.Models
{
    [DataContract]
    public class NewsMessageModel : RequestMessageModel
    {
        [DataMember(Name = "articles")]
        public IList<NewsArticleModel> Articles { get; set; }
    }

    [DataContract]
    public class NewsArticleModel
    {
        public NewsArticleModel()
        {
        }

        public NewsArticleModel(string title, string description, string url, string pictureUrl)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            PictureUrl = pictureUrl ?? throw new ArgumentNullException(nameof(pictureUrl));
        }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "picurl")]
        public string PictureUrl { get; set; }
    }
}
