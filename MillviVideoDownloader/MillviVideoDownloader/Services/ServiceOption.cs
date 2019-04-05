using System;
using Microsoft.Extensions.Configuration;

namespace MillviVideoDownloader.Services
{
    public class ServiceOption
    {
        public Uri LoginPageUrl { get; set; }

        public static ServiceOption FromConfiguration(IConfiguration config)
        {
            return new ServiceOption
            {
                LoginPageUrl = new Uri(config["loginPageUrl"])
            };
        }
    }
}