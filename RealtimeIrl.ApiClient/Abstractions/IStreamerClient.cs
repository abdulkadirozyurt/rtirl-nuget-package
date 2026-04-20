
using RealtimeIrl.ApiClient.Models;

namespace RealtimeIrl.ApiClient.Abstractions;

/// <summary>
/// Client for listening to a streamer's public data.
/// </summary>
public interface IStreamerClient : IClient
{
    IDisposable AddLocationListener(Action<Location?> callback);
}
