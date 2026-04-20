namespace RealtimeIrl.ApiClient.Abstractions;

/// <summary>
/// Factory for creating RTIRL clients.
/// </summary>
public interface IRealtimeIRLClientFactory
{
    IPullKeyClient ForPullKey(string pullKey);
    IStreamerClient ForStreamer(string provider, string userId);
}
