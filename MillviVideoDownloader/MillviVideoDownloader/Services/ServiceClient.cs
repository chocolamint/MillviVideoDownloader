using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace MillviVideoDownloader.Services
{
    public class ServiceClient
    {
        private readonly ServiceOption _option;
        private readonly HttpClient _httpClient;
        private readonly HtmlParser _htmlParser = new HtmlParser();

        public ServiceClient(ServiceOption option)
        : this(option, new HttpClientHandler { UseCookies = true })
        {

        }

        private ServiceClient(ServiceOption option, HttpMessageHandler httpMessageHandler)
        {
            _option = option;
            _httpClient = new HttpClient(httpMessageHandler);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36");
        }

        public async Task LoginAsync(string userId, string password, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(_option.LoginPageUrl, cancellationToken);
            var html = await response.Content.ReadAsStringAsync();
            var doc = await _htmlParser.ParseDocumentAsync(html, cancellationToken);
            var form = doc.QuerySelector("form");
            var @params = form.QuerySelectorAll("input,select,textarea")
                              .Where(x => x.Attributes["name"] != null)
                              .Select(x => ToKeyValuePair(x))
                              .Select(x => FillAuthenticationInfo(x));

            var postUri = new Uri(_option.LoginPageUrl, form.Attributes["action"].Value);

            response = await _httpClient.PostAsync(postUri, new FormUrlEncodedContent(@params), cancellationToken);

            response.EnsureSuccessStatusCode();

            // TODO: Ensure login success.

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
        }
    }
}
