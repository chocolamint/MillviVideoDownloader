using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MillviVideoDownloader.Services
{
    public class Ffmpeg : IFfmpeg
    {
        private readonly string _ffmpegPath;

        public Ffmpeg(string ffmpegPath)
        {
            _ffmpegPath = ffmpegPath;
        }

        public Task ExecuteAsync(string input, string output, CancellationToken cancellationToken = default)
        {
            var arguments = $"-protocol_whitelist file,http,https,tcp,tls,crypto " +
                            $"-i {EscapePath(input)} " +
                            $"-movflags faststart " +
                            $"-c copy " +
                            $"{EscapePath(output)}";

            var taskCompletionSource = new TaskCompletionSource<object>();
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ffmpegPath,
                    Arguments = arguments,
                    //CreateNoWindow = true,
                    //UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(_ffmpegPath),
                    //RedirectStandardInput = true,
                    //RedirectStandardOutput = true,
                    //RedirectStandardError = true
                },
                EnableRaisingEvents = true,
            };

            process.Exited += OnProcessExited;
            process.Start();

            return taskCompletionSource.Task;

            void OnProcessExited(object sender, EventArgs e)
            {
                process.Exited -= OnProcessExited;
                var exitCode = process.ExitCode;
                var errorMessage = exitCode == 0 ? null : process.StandardError.ReadToEnd();
                process.Dispose();

                if (exitCode != 0)
                {
                    taskCompletionSource.TrySetException(new Exception(errorMessage));
                }
                else
                {
                    taskCompletionSource.TrySetResult(null);
                }
            }
        }

        private string EscapePath(string path) => "\"" + path.Replace("\\", "\\\\") + "\"";
    }
}