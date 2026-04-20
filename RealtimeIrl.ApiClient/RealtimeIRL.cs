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

    /// <summary>
    /// Firebase configuration used by this app instance.
    /// </summary>
    public FirebaseAppConfig Config { get; }

    /// <summary>
    /// Creates a RealtimeIRL app instance with the specified Firebase configuration.
    /// </summary>
    public RealtimeIRLApp(FirebaseAppConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        Config = config;
        _factory = new RealtimeIRLClientFactory(new FirebaseConnectionFactory(config.DatabaseUrl));
    }

    /// <summary>
    /// Creates a client for private pull-key telemetry.
    /// </summary>
    public IPullKeyClient ForPullKey(string pullKey)
    {
        ThrowIfNullOrWhiteSpace(pullKey, nameof(pullKey));
        return _factory.ForPullKey(pullKey);
    }

    /// <summary>
    /// Creates a client for public streamer location data.
    /// </summary>
    public IStreamerClient ForStreamer(string provider, string userId)
    {
        ThrowIfNullOrWhiteSpace(provider, nameof(provider));
        ThrowIfNullOrWhiteSpace(userId, nameof(userId));
        return _factory.ForStreamer(provider, userId);
    }

    private static void ThrowIfNullOrWhiteSpace(string value, string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", paramName ?? nameof(value));
        }
    }
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
        ArgumentNullException.ThrowIfNull(config);

        _defaultApp = new RealtimeIRLApp(config);
        return _defaultApp;
    }

    private static RealtimeIRLApp GetDefaultApp()
    {
        return _defaultApp ??= InitializeApp(new FirebaseAppConfig());
    }

    /// <summary>
    /// Creates a client for private pull-key telemetry using the default app.
    /// </summary>
    public static IPullKeyClient ForPullKey(string pullKey) => GetDefaultApp().ForPullKey(pullKey);

    /// <summary>
    /// Creates a client for public streamer location data using the default app.
    /// </summary>
    public static IStreamerClient ForStreamer(string provider, string userId) => GetDefaultApp().ForStreamer(provider, userId);

    /// <summary>
    /// Listens for private pull-key location updates using the default app.
    /// </summary>
    public static IDisposable AddLocationListener(string pullKey, Action<Location> callback, Action<Exception>? onError = null)
        => ForPullKey(pullKey).AddLocationListener(callback, onError);

    /// <summary>
    /// Listens for private pull-key speed updates using the default app.
    /// </summary>
    public static IDisposable AddSpeedListener(string pullKey, Action<double> callback, Action<Exception>? onError = null)
        => ForPullKey(pullKey).AddSpeedListener(callback, onError);

    /// <summary>
    /// Listens for private pull-key heading updates using the default app.
    /// </summary>
    public static IDisposable AddHeadingListener(string pullKey, Action<double> callback, Action<Exception>? onError = null)
        => ForPullKey(pullKey).AddHeadingListener(callback, onError);

    /// <summary>
    /// Listens for private pull-key altitude updates using the default app.
    /// </summary>
    public static IDisposable AddAltitudeListener(string pullKey, Action<double> callback, Action<Exception>? onError = null)
        => ForPullKey(pullKey).AddAltitudeListener(callback, onError);

    /// <summary>
    /// Listens for private pull-key session id updates using the default app.
    /// </summary>
    public static IDisposable AddSessionIdListener(string pullKey, Action<string?> callback, Action<Exception>? onError = null)
        => ForPullKey(pullKey).AddSessionIdListener(callback, onError);
}
