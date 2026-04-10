using RtIrl.Api.Models;

namespace RtIrl.Api.Abstractions;

/// <summary>
/// Client for listening to a streamer's private data via a pull key.
/// </summary>
public interface IPullKeyClient : IDisposable
{
    IDisposable AddLocationListener(Action<Location> callback);
    IDisposable AddSpeedListener(Action<double> callback);
    IDisposable AddHeadingListener(Action<double> callback);
    IDisposable AddAltitudeListener(Action<double> callback);
    IDisposable AddHeartRateListener(Action<int> callback);
    IDisposable AddCyclingPowerListener(Action<int> callback);
    IDisposable AddPedometerStepsListener(Action<long> callback);
    IDisposable AddSessionIdListener(Action<string?> callback);
    IDisposable AddListener(Action<object?> callback);
}

/// <summary>
/// Client for listening to a streamer's public data.
/// </summary>
public interface IStreamerClient : IDisposable
{
    IDisposable AddLocationListener(Action<Location?> callback);
}
