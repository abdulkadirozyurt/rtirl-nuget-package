
using RealtimeIrl.ApiClient.Models;

namespace RealtimeIrl.ApiClient.Abstractions;

/// <summary>
/// Client for listening to a streamer's public data.
/// </summary>
public interface IStreamerClient : IClient
{
    /// <summary>Listens for public streamer location updates. The value is null when hidden or unavailable.</summary>
    IDisposable AddLocationListener(Action<Location?> callback, Action<Exception>? onError = null);
}
