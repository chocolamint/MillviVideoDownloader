using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MillviVideoDownloader.Services
{
    public class VideoInfo
    {
        private readonly Uri _playlistUri;
        private readonly HttpClient _httpClient;
        private readonly IFfmpeg _ffmpeg;

        public string FileName { get; set; }

        public VideoInfo(Uri playlistUri, HttpClient httpClient, IFfmpeg ffmpeg)
        {
            _playlistUri = playlistUri;
            _httpClient = httpClient;
            _ffmpeg = ffmpeg;
        }

        public async Task DownloadToAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var tempPlaylistPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".m3u8");
            var tempMp4Path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".mp4");

            try
            {
                using (var tempPlaylistStream = File.OpenWrite(tempPlaylistPath))
                using (var response = await _httpClient.GetAsync(_playlistUri, cancellationToken))
                using (var playlistDownloading = await response.Content.ReadAsStreamAsync())
                {
                    await playlistDownloading.CopyToAsync(tempPlaylistStream, cancellationToken);
                }

                await _ffmpeg.ExecuteAsync(tempPlaylistPath, tempMp4Path, cancellationToken);

                File.Move(tempMp4Path, filePath);
            }
            finally
            {
                try { File.Delete(tempPlaylistPath); } catch { }
                try { File.Delete(tempMp4Path); } catch { }
            }
        }
    }
}