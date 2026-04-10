# RtIrl.Api

.NET 10 Client API library for [rtirl.com](https://rtirl.com).

This is a .NET port of the official `@rtirl/api` npm package. It allows you to listen to real-time telemetry data (location, speed, heart rate, etc.) from IRL streamers using Firebase Realtime Database.

## Installation

Install via NuGet:

```bash
dotnet add package RtIrl.Api
```

## Complete Usage Guide

The API provides two main access methods: **Pull Key** (Private Data) and **Streamer ID** (Public Data).

### 1. Using Pull Key (Private, Full Telemetry Data)

A Pull Key allows access to all telemetry data broadcasted by the streamer's device. 

```csharp
using RtIrl.Api;
using RtIrl.Api.Models;

var pullKey = "YOUR_PULL_KEY";

// Initialize the client
using var client = RealtimeIRL.ForPullKey(pullKey);

// 1. Location (Latitude, Longitude)
client.AddLocationListener(loc => {
    Console.WriteLine($"Location: {loc.Latitude}, {loc.Longitude}");
});

// 2. Speed (Meters per second) - Returns double
client.AddSpeedListener(speed => {
    Console.WriteLine($"Speed: {speed} m/s");
});

// 3. Heading (Degrees) - Returns double
client.AddHeadingListener(heading => {
    Console.WriteLine($"Heading: {heading}°");
});

// 4. Altitude (Meters) - Returns double
client.AddAltitudeListener(altitude => {
    Console.WriteLine($"Altitude: {altitude} m");
});

// 5. Heart Rate (Beats per minute) - Returns int
client.AddHeartRateListener(bpm => {
    Console.WriteLine($"Heart Rate: {bpm} BPM");
});

// 6. Cycling Power (Watts) - Returns int
client.AddCyclingPowerListener(watts => {
    Console.WriteLine($"Power: {watts} W");
});

// 7. Pedometer Steps (Total steps) - Returns long
// Note: This number is monotonically increasing for a given session.
client.AddPedometerStepsListener(steps => {
    Console.WriteLine($"Steps taken: {steps}");
});

// 8. Session ID (UUID) - Returns string?
// Triggered when a new session starts. Returns null if the streamer is offline.
client.AddSessionIdListener(sessionId => {
    if (sessionId != null)
        Console.WriteLine($"Online! Session ID: {sessionId}");
    else
        Console.WriteLine("Streamer went offline.");
});

// 9. Raw Data / Unstructured Data - Returns object?
// Listen to the entire pull key node including telemetry not mapped to typed properties.
client.AddListener(data => {
    Console.WriteLine($"Raw Firebase Node Data: {data}");
});

// Keep the console application running to listen
Console.ReadLine();
```

### 2. Using Streamer ID (Public Location Data)

If you only know the streamer's public platform ID (e.g., Twitch user ID), you can only access their public location. This location might be hidden if the streamer is offline or has chosen to restrict it.

```csharp
using RtIrl.Api;

// Provider: "twitch", UserId: "463756153"
using var client = RealtimeIRL.ForStreamer("twitch", "463756153");

client.AddLocationListener(loc => {
    if (loc != null)
        Console.WriteLine($"Public Location: {loc.Latitude}, {loc.Longitude}");
    else
        Console.WriteLine("Location is hidden or streamer is offline.");
});

Console.ReadLine();
```

## Unsubscribing from Listeners (Cleanup)

Under the hood, this library uses WebSockets. Every `Add*Listener` method returns an `IDisposable`. To stop listening to a specific event and free up resources, simply call `.Dispose()` on the returned object. This prevents memory leaks and closes the network connection for that specific node.

```csharp
var speedSubscription = client.AddSpeedListener(speed => {
    Console.WriteLine($"Speed: {speed} m/s");
});

// Later in your code, when you no longer need speed updates:
speedSubscription.Dispose();
```

Alternatively, disposing the entire `client` instance (e.g., via the `using` statement or calling `client.Dispose()`) will automatically unsubscribe **all** active listeners attached to that client.

## Features

- **Real-time:** Uses WebSockets (via `FirebaseDatabase.net`) for low-latency updates.
- **Clean Architecture:** Built with SOLID principles, highly modular, and testable.
- **Easy Cleanup:** All listeners return `IDisposable` for granular or bulk memory management.
- **Modern .NET:** Targeted for .NET 10.

## License

MIT
