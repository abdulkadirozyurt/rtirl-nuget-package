using RealtimeIrl.ApiClient.Models;

namespace RealtimeIrl.ApiClient.Abstractions;

/// <summary>
/// Client for listening to a streamer's private data via a pull key.
/// </summary>
public interface IPullKeyClient : IClient
{
    /// <summary>Listens for private location updates for the pull key.</summary>
    IDisposable AddLocationListener(Action<Location> callback, Action<Exception>? onError = null);

    /// <summary>Listens for speed updates in meters per second.</summary>
    IDisposable AddSpeedListener(Action<double> callback, Action<Exception>? onError = null);

    /// <summary>Listens for heading updates in degrees.</summary>
    IDisposable AddHeadingListener(Action<double> callback, Action<Exception>? onError = null);

    /// <summary>Listens for altitude updates in meters.</summary>
    IDisposable AddAltitudeListener(Action<double> callback, Action<Exception>? onError = null);

    /// <summary>Listens for heart rate updates in beats per minute.</summary>
    IDisposable AddHeartRateListener(Action<int> callback, Action<Exception>? onError = null);

    /// <summary>Listens for cycling power updates in watts.</summary>
    IDisposable AddCyclingPowerListener(Action<int> callback, Action<Exception>? onError = null);

    /// <summary>Listens for cycling crank updates.</summary>
    IDisposable AddCyclingCrankListener(Action<int> callback, Action<Exception>? onError = null);

    /// <summary>Listens for cycling wheel updates.</summary>
    IDisposable AddCyclingWheelListener(Action<int> callback, Action<Exception>? onError = null);

    /// <summary>Listens for pedometer step count updates.</summary>
    IDisposable AddPedometerStepsListener(Action<long> callback, Action<Exception>? onError = null);

    /// <summary>Listens for active session id updates.</summary>
    IDisposable AddSessionIdListener(Action<string?> callback, Action<Exception>? onError = null);

    /// <summary>Listens for raw pull-key node updates.</summary>
    IDisposable AddListener(Action<object?> callback, Action<Exception>? onError = null);
}
