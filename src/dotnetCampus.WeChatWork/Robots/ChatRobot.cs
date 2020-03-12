using dotnetCampus.WeChatWork.Robots.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// 创建一个企业微信群聊机器人。
        /// </summary>
        /// <param name="webHookUrl">在创建群聊机器人时会提示你 WebHoolUrl，在此传入即可。</param>
        public ChatRobot(string webHookUrl)
        {
            _webHookUrl = webHookUrl ?? throw new ArgumentNullException(nameof(webHookUrl));
        }

        /// <summary>
        /// 发送纯文本消息。
        /// </summary>
        /// <param name="text">要发送的纯文本消息正文。</param>
        /// <param name="mentions">要额外提及这些人。应该使用企业微信 Id 或者手机号，而不是姓名；可混合传入，会自动识别。</param>
        /// <returns>发送消息后的服务器响应。</returns>
        public Task<ResponseModel> SendTextAsync(string text, params string[] mentions)
        {
            var idList = mentions.Where(x => !x.All(l => l >= '0' && l <= '9')).ToList();
            var phoneList = mentions.Where(x => x.All(l => l >= '0' && l <= '9')).ToList();
            return SendMessageAsync(new
            {
                msgtype = "text",
                text = new TextMessageModel
                {
                    Content = text,
                    MentionedList = mentions,
                    MentionedMobileList = phoneList,
                }
            });
        }

        /// <summary>
        /// 发送简化版的 Markdown 消息（企业微信仅支持 Markdown 子集，具体可参见企业微信创建机器人后的详情页）。
        /// </summary>
        /// <param name="markdown">要发送的 Markdown 消息正文。</param>
        /// <returns>发送消息后的服务器响应。</returns>
        public Task<ResponseModel> SendMarkdownAsync(string markdown)
            => SendMessageAsync(new
            {
                msgtype = "markdown",
                markdown = new MarkdownMessageModel
                {
                    Content = markdown,
                }
            });

        /// <summary>
        /// 发送 Base64 数据格式的图片消息。
        /// </summary>
        /// <param name="base64">Base64 数据正文。</param>
        /// <param name="md5">图片文件的 md5 校验值。</param>
        /// <returns>发送消息后的服务器响应。</returns>
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

        /// <summary>
        /// 发送图文消息。
        /// </summary>
        /// <param name="title">消息标题。</param>
        /// <param name="description">消息简介。</param>
        /// <param name="url">消息网址。</param>
        /// <param name="pictureUrl">附图网址。</param>
        /// <returns>发送消息后的服务器响应。</returns>
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

        /// <summary>
        /// 发送任意格式的企业微信消息。如果你希望发送的消息超出本机器人提供的范围（例如企业微信更新但此库没有更新），则使用此方法发送任意格式的消息。
        /// </summary>
        /// <typeparam name="T">要发送的消息类型。</typeparam>
        /// <param name="message">要发送的消息体。</param>
        /// <returns>发送消息后的服务器响应。</returns>
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
