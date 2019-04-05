using System;
using Microsoft.Extensions.Configuration;

namespace MillviVideoDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var url = config["loginPageUrl"];
        }
    }
}
