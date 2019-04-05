using System;
using Microsoft.Extensions.Configuration;

namespace MillviVideoDownloader.Services
{
    public class ServiceOption
    {
        public Uri LoginPageUri { get; set; }
        public Uri VideoPageUri { get; set; }

        public static ServiceOption FromConfiguration(IConfiguration config)
        {
            return new ServiceOption
            {
                LoginPageUri = new Uri(config["loginPageUrl"]),
                VideoPageUri = new Uri(config["videoPageUrl"])
            };
        }
    }
}