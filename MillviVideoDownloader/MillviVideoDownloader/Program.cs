using System.IO;
using System.Reflection;
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
            var entryAssembly = Assembly.GetEntryAssembly();
            var executingDirectory = Path.GetDirectoryName(entryAssembly.Location);

            var userId = config["u"];
            var password = config["p"];
            var downloadDirectory = config["downloadDirectory"];

            var serviceOption = ServiceOption.FromConfiguration(config);
            var ffmpeg = new Ffmpeg(Path.Combine(executingDirectory, "ffmpeg.exe"));
            var client = new ServiceClient(serviceOption, ffmpeg);

            await client.LoginAsync(userId, password);

            var video = await client.GetVideoAsync();
            var filePath = Path.Combine(downloadDirectory, video.FileName);
            if (!File.Exists(filePath))
            {
                await video.DownloadToAsync(filePath);
            }
        }
    }
}
