using RealtimeIrl.ApiClient.Abstractions;
using RealtimeIrl.ApiClient.Infrastructure.Abstractions;

namespace RealtimeIrl.ApiClient.Internal;

/// <summary>
/// Internal implementation of the RTIRL client factory.
/// </summary>
internal class RealtimeIRLClientFactory : IRealtimeIRLClientFactory
{
    private readonly IFirebaseConnectionFactory _connectionFactory;

    public RealtimeIRLClientFactory(IFirebaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IPullKeyClient ForPullKey(string pullKey)
    {
        return new PullKeyClient(_connectionFactory.CreateClient(), pullKey);
    }

    public IStreamerClient ForStreamer(string provider, string userId)
    {
        return new StreamerClient(_connectionFactory.CreateClient(), provider, userId);
    }
}
