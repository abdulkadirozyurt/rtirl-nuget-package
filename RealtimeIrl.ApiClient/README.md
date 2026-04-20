# RealtimeIrl.ApiClient

.NET API client library for [rtirl.com](https://rtirl.com). It provides a typed .NET wrapper around the RealtimeIRL Firebase Realtime Database streams, based on the official `@rtirl/api` npm package.

## Requirements

- .NET 8 or later
- A RealtimeIRL pull key for private telemetry, or a public streamer provider/user id pair for public location data

## Installation

```bash
dotnet add package RealtimeIrl.ApiClient
```

## Pull Key Telemetry

A pull key gives access to the private telemetry node for a streamer.

```csharp
using RealtimeIrl.ApiClient;

using var client = RealtimeIRL.ForPullKey("YOUR_PULL_KEY");

using var locationSub = client.AddLocationListener(location =>
{
    Console.WriteLine($"Location: {location.Latitude}, {location.Longitude}");
});

using var speedSub = client.AddSpeedListener(speed =>
{
    Console.WriteLine($"Speed: {speed} m/s");
});

Console.ReadLine();
```

Available pull-key listeners:

- `AddLocationListener(Action<Location> callback)`
- `AddSpeedListener(Action<double> callback)`
- `AddHeadingListener(Action<double> callback)`
- `AddAltitudeListener(Action<double> callback)`
- `AddHeartRateListener(Action<int> callback)`
- `AddCyclingPowerListener(Action<int> callback)`
- `AddCyclingCrankListener(Action<int> callback)`
- `AddCyclingWheelListener(Action<int> callback)`
- `AddPedometerStepsListener(Action<long> callback)`
- `AddSessionIdListener(Action<string?> callback)`
- `AddListener(Action<object?> callback)` for raw pull-key node updates

## Public Streamer Location

Public streamer access only exposes public location data. The callback may receive `null` when the streamer is offline or location is hidden.

```csharp
using RealtimeIrl.ApiClient;

using var client = RealtimeIRL.ForStreamer("twitch", "463756153");

using var sub = client.AddLocationListener(location =>
{
    if (location is null)
    {
        Console.WriteLine("Location is hidden or unavailable.");
        return;
    }

    Console.WriteLine($"Public location: {location.Latitude}, {location.Longitude}");
});

Console.ReadLine();
```

## Error Handling

Every listener accepts an optional error callback. The library does not write Firebase errors to `Console`.

```csharp
using var sub = client.AddLocationListener(
    location => Console.WriteLine($"{location.Latitude}, {location.Longitude}"),
    error => Console.Error.WriteLine(error.Message));
```

## Custom Firebase Database URL

The default configuration points at the RealtimeIRL Firebase database. If needed, initialize a custom app before creating clients.

```csharp
using RealtimeIrl.ApiClient;
using RealtimeIrl.ApiClient.Models;

RealtimeIRL.InitializeApp(new FirebaseAppConfig
{
    DatabaseUrl = "https://your-database.firebaseio.com"
});
```

## Cleanup

Each listener returns an `IDisposable`. Dispose individual subscriptions to stop a listener, or dispose the client to stop all listeners created by that client.

```csharp
var speedSubscription = client.AddSpeedListener(speed =>
{
    Console.WriteLine($"Speed: {speed} m/s");
});

speedSubscription.Dispose();
```

## Notes

- Firebase Analytics event logging from the npm package is not implemented in this .NET client.
- `FirebaseAppConfig` currently uses only `DatabaseUrl`.
- The package targets `net8.0`, so it can be consumed by .NET 8 and newer projects.

## License

Apache-2.0
