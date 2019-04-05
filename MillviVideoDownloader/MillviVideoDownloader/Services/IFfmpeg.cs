using System.Threading;
using System.Threading.Tasks;

namespace MillviVideoDownloader.Services
{
    public interface IFfmpeg
    {
        Task ExecuteAsync(string input, string output, CancellationToken cancellationToken = default);
    }
}