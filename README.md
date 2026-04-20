# RealtimeIRL API Client for .NET

Typed .NET client for [rtirl.com](https://rtirl.com) real-time telemetry streams.

[![NuGet](https://img.shields.io/nuget/v/RealtimeIrl.ApiClient?label=NuGet)](https://www.nuget.org/packages/RealtimeIrl.ApiClient)
[![Target Framework](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue)](https://www.apache.org/licenses/LICENSE-2.0)
[![Source](https://img.shields.io/badge/source-GitHub-181717)](https://github.com/abdulkadirozyurt/rtirl-nuget-package)

## At A Glance

| You need | Use this | Data exposed |
| --- | --- | --- |
| Private streamer telemetry | `RealtimeIRL.ForPullKey(...)` | Location, speed, heading, altitude, heart rate, cycling, pedometer, session id |
| Public streamer location | `RealtimeIRL.ForStreamer(...)` | Public location only |
| Custom Firebase database | `RealtimeIRL.InitializeApp(...)` | Same clients, custom database URL |

```text
RealtimeIRL
   |
   +-- ForPullKey("pull-key") --------> private telemetry listeners
   |
   +-- ForStreamer("twitch", "id") ---> public location listener
```

## Installation

```bash
dotnet add package RealtimeIrl.ApiClient
```

## Quick Start

### Private Telemetry With A Pull Key

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

### Public Streamer Location

Public streamer access returns only public location data. The callback can receive `null` when the streamer is offline, hidden, or unavailable.

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

## Listener Matrix

| Listener | Pull key client | Streamer client | Callback value |
| --- | :---: | :---: | --- |
| `AddLocationListener` | Yes | Yes | `Location` / `Location?` |
| `AddSpeedListener` | Yes | No | `double` meters per second |
| `AddHeadingListener` | Yes | No | `double` degrees |
| `AddAltitudeListener` | Yes | No | `double` meters |
| `AddHeartRateListener` | Yes | No | `int` beats per minute |
| `AddCyclingPowerListener` | Yes | No | `int` watts |
| `AddCyclingCrankListener` | Yes | No | `int` |
| `AddCyclingWheelListener` | Yes | No | `int` |
| `AddPedometerStepsListener` | Yes | No | `long` |
| `AddSessionIdListener` | Yes | No | `string?` |
| `AddListener` | Yes | No | `object?` raw pull-key node |

## Error Handling

Every listener accepts an optional error callback. Firebase errors are passed to your code instead of being written to `Console`.

```csharp
using var sub = client.AddLocationListener(
    location => Console.WriteLine($"{location.Latitude}, {location.Longitude}"),
    error => Console.Error.WriteLine(error.Message));
```

## Cleanup

Listeners return `IDisposable`.

| Dispose this | Result |
| --- | --- |
| Listener subscription | Stops one listener |
| Client instance | Stops all listeners created by that client |

```csharp
var speedSubscription = client.AddSpeedListener(speed =>
{
    Console.WriteLine($"Speed: {speed} m/s");
});

speedSubscription.Dispose();
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

## Repository Structure

| Path | Purpose |
| --- | --- |
| `RealtimeIrl.ApiClient/` | NuGet package source |
| `RealtimeIrl.ApiClient.Tests/` | Unit tests |
| `artifacts/packages/` | Local package output |

## Requirements

| Requirement | Value |
| --- | --- |
| Target framework | `.NET 8.0` or later |
| Package id | `RealtimeIrl.ApiClient` |
| License | `Apache-2.0` |
| Dependency | `FirebaseDatabase.net` |

## Notes

| Topic | Status |
| --- | --- |
| Firebase Analytics from the npm package | Not implemented |
| `FirebaseAppConfig` | Currently uses only `DatabaseUrl` |
| Real-time stream transport | Firebase Realtime Database |

## License

Apache-2.0
