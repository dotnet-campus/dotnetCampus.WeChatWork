using dotnetCampus.WeChatWork.Robots.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace dotnetCampus.WeChatWork.Robots
{
    /// <summary>
    /// 表示一个企业微信中的普通群聊机器人。
    /// 普通群聊机器人指的是任何人都可以在群里创建的机器人。
    /// </summary>
    public sealed class ChatRobot
    {
        private readonly string _webHookUrl;

        public ChatRobot(string webHookUrl)
        {
            _webHookUrl = webHookUrl ?? throw new ArgumentNullException(nameof(webHookUrl));
        }

        public Task<ResponseModel> SendTextAsync(string text, params string[] mentions)
            => SendMessageAsync(new
            {
                msgtype = "text",
                text = new TextMessageModel
                {
                    Content = text,
                    MentionedList = mentions,
                }
            });

        public Task<ResponseModel> SendMarkdownAsync(string markdown)
            => SendMessageAsync(new
            {
                msgtype = "markdown",
                markdown = new MarkdownMessageModel
                {
                    Content = markdown,
                }
            });

        public Task<ResponseModel> SendImageFromBase64Async(string base64, string md5)
            => SendMessageAsync(new
            {
                msgtype = "image",
                image = new ImageMessageModel
                {
                    Base64 = base64,
                    Md5 = md5,
                }
            });

        public Task<ResponseModel> SendNewsAsync(string title, string description, string url, string pictureUrl)
            => SendMessageAsync(new
            {
                msgtype = "news",
                news = new NewsMessageModel
                {
                    Articles = new List<NewsArticleModel>
                    {
                        new NewsArticleModel(title, description, url, pictureUrl),
                    }
                }
            });

        public async Task<ResponseModel> SendMessageAsync<T>(T message)
        {
            var requestString = JsonConvert.SerializeObject(message);
            var stringContent = new StringContent(requestString, Encoding.UTF8, "application/json");

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(_webHookUrl, stringContent).ConfigureAwait(false);

            if (response.StatusCode >= HttpStatusCode.OK && response.StatusCode < HttpStatusCode.MultipleChoices)
            {
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseObject = JsonConvert.DeserializeObject<ResponseModel>(responseString);
                return responseObject;
            }
            else
            {
                return new ResponseModel
                {
                    ErrorCode = (int)response.StatusCode,
                    ErrorMessage = response.StatusCode.ToString(),
                };
            }
        }
    }
}
