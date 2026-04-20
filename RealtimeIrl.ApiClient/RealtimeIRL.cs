using RealtimeIrl.ApiClient.Abstractions;
using RealtimeIrl.ApiClient.Infrastructure;
using RealtimeIrl.ApiClient.Internal;
using RealtimeIrl.ApiClient.Models;


namespace RealtimeIrl.ApiClient;

/// <summary>
/// Represents a Firebase application instance for RTIRL.
/// </summary>
public class RealtimeIRLApp
{
    private readonly IRealtimeIRLClientFactory _factory;
    public FirebaseAppConfig Config { get; }

    public RealtimeIRLApp(FirebaseAppConfig config)
    {
        Config = config;
        _factory = new RealtimeIRLClientFactory(new FirebaseConnectionFactory(config.DatabaseUrl));
    }

    public IPullKeyClient ForPullKey(string pullKey) => _factory.ForPullKey(pullKey);
    public IStreamerClient ForStreamer(string provider, string userId) => _factory.ForStreamer(provider, userId);
}

/// <summary>
/// Static entry point for RTIRL API, mimicking the JS API structure.
/// </summary>
public static class RealtimeIRL
{
    private static RealtimeIRLApp? _defaultApp;

    /// <summary>
    /// Initializes the default RTIRL application with the provided configuration.
    /// </summary>
    public static RealtimeIRLApp InitializeApp(FirebaseAppConfig config)
    {
        _defaultApp = new RealtimeIRLApp(config);
        return _defaultApp;
    }

    private static RealtimeIRLApp GetDefaultApp()
    {
        return _defaultApp ??= InitializeApp(new FirebaseAppConfig());
    }

    public static IPullKeyClient ForPullKey(string pullKey) => GetDefaultApp().ForPullKey(pullKey);
    public static IStreamerClient ForStreamer(string provider, string userId) => GetDefaultApp().ForStreamer(provider, userId);

    public static IDisposable AddLocationListener(string pullKey, Action<Location> callback)
        => ForPullKey(pullKey).AddLocationListener(callback);

    public static IDisposable AddSpeedListener(string pullKey, Action<double> callback)
        => ForPullKey(pullKey).AddSpeedListener(callback);

    public static IDisposable AddHeadingListener(string pullKey, Action<double> callback)
        => ForPullKey(pullKey).AddHeadingListener(callback);

    public static IDisposable AddAltitudeListener(string pullKey, Action<double> callback)
        => ForPullKey(pullKey).AddAltitudeListener(callback);

    public static IDisposable AddSessionIdListener(string pullKey, Action<string?> callback)
        => ForPullKey(pullKey).AddSessionIdListener(callback);
}
