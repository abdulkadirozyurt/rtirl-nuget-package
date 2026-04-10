using RtIrl.Api.Abstractions;
using RtIrl.Api.Infrastructure;
using RtIrl.Api.Internal;
using RtIrl.Api.Models;

namespace RtIrl.Api;

/// <summary>
/// Represents a Firebase application instance for RTIRL.
/// </summary>
public class RtIrlApp
{
    private readonly IRtIrlClientFactory _factory;
    public FirebaseAppConfig Config { get; }

    public RtIrlApp(FirebaseAppConfig config)
    {
        Config = config;
        // Fabrika artık konfigürasyon almadığı için parametresiz çağırıyoruz.
        // Eğer ileride URL değişirse fabrika güncellenebilir.
        _factory = new RtIrlClientFactory(new RtIrlFirebaseConnectionFactory());
    }

    public IPullKeyClient ForPullKey(string pullKey) => _factory.ForPullKey(pullKey);
    public IStreamerClient ForStreamer(string provider, string userId) => _factory.ForStreamer(provider, userId);
}

/// <summary>
/// Static entry point for RTIRL API, mimicking the JS API structure.
/// </summary>
public static class RealtimeIRL
{
    private static RtIrlApp? _defaultApp;

    /// <summary>
    /// Initializes the default RTIRL application with the provided configuration.
    /// </summary>
    public static RtIrlApp InitializeApp(FirebaseAppConfig config)
    {
        _defaultApp = new RtIrlApp(config);
        return _defaultApp;
    }

    private static RtIrlApp GetDefaultApp()
    {
        return _defaultApp ??= InitializeApp(new FirebaseAppConfig());
    }

    public static IPullKeyClient ForPullKey(string pullKey) => GetDefaultApp().ForPullKey(pullKey);
    public static IStreamerClient ForStreamer(string provider, string userId) => GetDefaultApp().ForStreamer(provider, userId);
}
