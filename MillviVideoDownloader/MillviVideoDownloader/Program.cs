using System;
using Microsoft.Extensions.Configuration;

namespace MillviVideoDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddCommandLine(args)
                                                   .AddJsonFile("appsettings.json")
                                                   .Build();

            var url = config["loginPageUrl"];
            var userId = config["u"];
            var password = config["p"];
        }
    }
}
