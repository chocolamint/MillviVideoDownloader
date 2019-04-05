using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MillviVideoDownloader.Services
{
    public class ServiceClient
    {
        private readonly ServiceOption _option;
        private readonly HttpClient _httpClient;

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
        }
    }
}
