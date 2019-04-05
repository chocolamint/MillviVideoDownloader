using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MillviVideoDownloader.Services
{
    public class ServiceClient
    {
        private static readonly Regex _embedKeyRegex = new Regex(@"embedkey\s*=\s*""([^\""]+)""");
        private static readonly Regex _paramRegex = new Regex(@"^[A-Za-z0-9_]+\((.+)\);$");

        private readonly ServiceOption _option;
        private readonly IFfmpeg _ffmpeg;
        private readonly HttpClient _httpClient;
        private readonly HtmlParser _htmlParser = new HtmlParser();

        public ServiceClient(ServiceOption option, IFfmpeg ffmpeg)
        : this(option, ffmpeg, new HttpClientHandler { UseCookies = true })
        {

        }

        private ServiceClient(ServiceOption option, IFfmpeg ffmpeg, HttpMessageHandler httpMessageHandler)
        {
            _option = option;
            _ffmpeg = ffmpeg;
            _httpClient = new HttpClient(httpMessageHandler);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36");
        }

        public async Task LoginAsync(string userId, string password, CancellationToken cancellationToken = default)
        {
            // Load login page.
            var response = await _httpClient.GetAsync(_option.LoginPageUri, cancellationToken);
            var html = await response.Content.ReadAsStringAsync();
            var doc = await _htmlParser.ParseDocumentAsync(html, cancellationToken);
            var form = doc.QuerySelector("form");
            var @params = form.QuerySelectorAll("input,select,textarea")
                              .Where(x => x.Attributes["name"] != null)
                              .Select(x => ToKeyValuePair(x))
                              .Select(x => FillAuthenticationInfo(x));

            var postUri = new Uri(_option.LoginPageUri, form.Attributes["action"].Value);

            // Post to login.
            response = await _httpClient.PostAsync(postUri, new FormUrlEncodedContent(@params), cancellationToken);

            response.EnsureSuccessStatusCode();

            // TODO: Ensure login success.

            #region local functions
            KeyValuePair<string, string> FillAuthenticationInfo(KeyValuePair<string, string> param)
            {
                string value;
                if (!string.IsNullOrEmpty(param.Value))
                {
                    value = param.Value;
                }
                else if (param.Key.Contains("user", StringComparison.OrdinalIgnoreCase))
                {
                    value = userId;
                }
                else if (param.Key.Contains("pass", StringComparison.OrdinalIgnoreCase))
                {
                    value = password;
                }
                else
                {
                    value = param.Value;
                }

                return new KeyValuePair<string, string>(param.Key, value);
            }

            KeyValuePair<string, string> ToKeyValuePair(IElement element)
            {
                return new KeyValuePair<string, string>(element.Attributes["name"].Value, element.Attributes["value"]?.Value);
            }
            #endregion
        }

        public async Task<VideoInfo> GetVideoAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(_option.VideoPageUri, cancellationToken);
            var html = await response.Content.ReadAsStringAsync();
            var embedKey = _embedKeyRegex.Match(html).Groups[1].Value;
            var initResponse = await InitializeAsync(embedKey, _option.VideoPageUri, cancellationToken);
            var playlistUrl = await GetPlaylistUrlAsync(initResponse, cancellationToken);

            return new VideoInfo(new Uri(playlistUrl), _httpClient, _ffmpeg)
            {
                FileName = _option.FileNameFormatRegex.Match(html).Result(_option.FileNameFormat)
            };
        }

        private async Task<InitResponse> InitializeAsync(string embedKey, Uri referer, CancellationToken cancellationToken)
        {
            var uri = $"https://cc.miovp.com/init?embedkey={Uri.EscapeUriString(embedKey)}&" +
                      "is_in_iframe=false&" +
                      "flash=0&" +
                      $"refererurl={Uri.EscapeUriString(referer.ToString())}&" +
                      "callback=Millvi004091543267920095_1553859772468";
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.TryAddWithoutValidation("Referer", referer.ToString());

            var response = await _httpClient.SendAsync(request, cancellationToken);

            var initJs = await response.Content.ReadAsStringAsync();
            var initJson = _paramRegex.Match(initJs).Groups[1].Value;
            var init = JsonConvert.DeserializeObject<InitResponse>(initJson);

            return init;
        }

        private async Task<string> GetPlaylistUrlAsync(InitResponse init, CancellationToken cancellationToken)
        {
            var url = $"https://cc.miovp.com/get_info?" +
                      $"host={init.Host}&" +
                      $"id_vhost={init.VhostId}" +
                      $"&id_contents={init.ContentId}&" +
                      $"videotype={init.VideoType}&" +
                      $"callback=Millvi012903781839485107_1553859772954";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            var getInfoJs = await response.Content.ReadAsStringAsync();

            var getInfoJson = _paramRegex.Match(getInfoJs).Groups[1].Value;

            var playlistUrl = JsonConvert.DeserializeAnonymousType(getInfoJson, new { url = default(string) }).url;
            return playlistUrl;
        }

        private class InitResponse
        {
            [JsonProperty("host")]
            public string Host { get; set; }

            [JsonProperty("id_vhost")]
            public int VhostId { get; set; }

            [JsonProperty("videotype")]
            public string VideoType { get; set; }

            [JsonProperty("id_contents")]
            public int ContentId { get; set; }
        }
    }
}
