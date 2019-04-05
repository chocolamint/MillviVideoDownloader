using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace MillviVideoDownloader.Services
{
    public class ServiceOption
    {
        public Uri LoginPageUri { get; set; }
        public Uri VideoPageUri { get; set; }
        public Regex FileNameFormatRegex { get; set; }
        public string FileNameFormat { get; set; }

        public static ServiceOption FromConfiguration(IConfiguration config)
        {
            return new ServiceOption
            {
                LoginPageUri = new Uri(config["loginPageUrl"]),
                VideoPageUri = new Uri(config["videoPageUrl"]),
                FileNameFormatRegex = new Regex(config["fileNameFormat:0"]),
                FileNameFormat = config["fileNameFormat:1"]
            };
        }
    }
}