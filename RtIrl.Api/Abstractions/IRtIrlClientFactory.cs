using RtIrl.Api.Abstractions;

namespace RtIrl.Api.Abstractions;

/// <summary>
/// Factory for creating RTIRL clients.
/// </summary>
public interface IRtIrlClientFactory
{
    IPullKeyClient ForPullKey(string pullKey);
    IStreamerClient ForStreamer(string provider, string userId);
}
