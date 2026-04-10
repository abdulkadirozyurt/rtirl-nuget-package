using RtIrl.Api.Abstractions;
using RtIrl.Api.Infrastructure;

namespace RtIrl.Api.Internal;

/// <summary>
/// Internal implementation of the RTIRL client factory.
/// </summary>
internal class RtIrlClientFactory : IRtIrlClientFactory
{
    private readonly IFirebaseConnectionFactory _connectionFactory;

    public RtIrlClientFactory(IFirebaseConnectionFactory connectionFactory)
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
