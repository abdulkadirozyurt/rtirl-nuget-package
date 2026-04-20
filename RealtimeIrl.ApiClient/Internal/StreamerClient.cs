using Firebase.Database;
using Firebase.Database.Query;
using RealtimeIrl.ApiClient.Abstractions;
using RealtimeIrl.ApiClient.Models;
using System.Reactive.Linq;

namespace RealtimeIrl.ApiClient.Internal;

internal class StreamerClient : IStreamerClient
{
    private sealed class CompositeSubscription : IDisposable
    {
        private readonly List<IDisposable> _inner;
        private bool _disposed;

        public CompositeSubscription(List<IDisposable> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            foreach (var sub in _inner)
            {
                sub.Dispose();
            }
        }
    }

    private readonly FirebaseClient _client;
    private readonly string _path;
    private readonly List<IDisposable> _subscriptions = new();

    public StreamerClient(FirebaseClient client, string provider, string userId)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        provider = string.IsNullOrWhiteSpace(provider)
            ? throw new ArgumentException("Provider cannot be null or empty.", nameof(provider))
            : provider;
        userId = string.IsNullOrWhiteSpace(userId)
            ? throw new ArgumentException("User ID cannot be null or empty.", nameof(userId))
            : userId;
        _path = $"{provider}:{userId}";
    }

    public IDisposable AddLocationListener(Action<Location?> callback, Action<Exception>? onError = null)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var locationQuery = _client
            .Child("streamers")
            .Child(_path)
            .Child("location");

        Location? lastEmitted = null;
        double? lastLatitude = null;
        double? lastLongitude = null;
        var hasEmitted = false;

        void EmitIfChanged(Location? next)
        {
            if (hasEmitted && EqualityComparer<Location?>.Default.Equals(lastEmitted, next))
            {
                return;
            }

            hasEmitted = true;
            lastEmitted = next;
            callback(next);
        }

        var locationSubscription = locationQuery
            .AsObservable<Location?>()
            .Subscribe(
                d =>
                {
                    var next = d.Object;
                    lastLatitude = next?.Latitude;
                    lastLongitude = next?.Longitude;
                    EmitIfChanged(next);
                },
                ex => onError?.Invoke(ex)
            );

        var latitudeSubscription = locationQuery
            .Child("latitude")
            .AsObservable<double?>()
            .Subscribe(
                d =>
                {
                    lastLatitude = d.Object;
                    if (lastLatitude is null || lastLongitude is null)
                    {
                        EmitIfChanged(null);
                        return;
                    }

                    EmitIfChanged(new Location(lastLatitude.Value, lastLongitude.Value));
                },
                ex => onError?.Invoke(ex)
            );

        var longitudeSubscription = locationQuery
            .Child("longitude")
            .AsObservable<double?>()
            .Subscribe(
                d =>
                {
                    lastLongitude = d.Object;
                    if (lastLatitude is null || lastLongitude is null)
                    {
                        EmitIfChanged(null);
                        return;
                    }

                    EmitIfChanged(new Location(lastLatitude.Value, lastLongitude.Value));
                },
                ex => onError?.Invoke(ex)
            );

        var composite = new CompositeSubscription(
            [locationSubscription, latitudeSubscription, longitudeSubscription]
        );
        _subscriptions.Add(composite);
        return composite;
    }

    public void Dispose()
    {
        foreach (var sub in _subscriptions) sub.Dispose();
        _subscriptions.Clear();
    }
}
