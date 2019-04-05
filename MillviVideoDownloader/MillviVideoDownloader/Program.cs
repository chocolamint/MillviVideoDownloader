using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MillviVideoDownloader.Services;

namespace MillviVideoDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddCommandLine(args)
                                                   .AddJsonFile("appsettings.json")
                                                   .Build();

            var userId = config["u"];
            var password = config["p"];

            var serviceOption = ServiceOption.FromConfiguration(config);
            var client = new ServiceClient(serviceOption);

            await client.LoginAsync(userId, password);
        }
    }
}
