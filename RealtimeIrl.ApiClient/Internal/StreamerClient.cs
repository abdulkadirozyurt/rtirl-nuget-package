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
        private readonly Action? _onDispose;
        private bool _disposed;

        public CompositeSubscription(List<IDisposable> inner, Action? onDispose = null)
        {
            _inner = inner;
            _onDispose = onDispose;
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

            _onDispose?.Invoke();
        }
    }

    private readonly FirebaseClient _client;
    private readonly string _path;
    private readonly List<IDisposable> _subscriptions = new();

    public StreamerClient(FirebaseClient client, string provider, string userId)
    {
        _client = client;
        _path = $"{provider}:{userId}";
    }

    public IDisposable AddLocationListener(Action<Location?> callback)
    {
        var locationQuery = _client
            .Child("streamers")
            .Child(_path)
            .Child("location");

        // npm onValue parity: subscription açıldığında anlık snapshot'ı da gönder.
        var lastEmitted = locationQuery.OnceSingleAsync<Location?>().GetAwaiter().GetResult();
        double? lastLatitude = lastEmitted?.Latitude;
        double? lastLongitude = lastEmitted?.Longitude;
        callback(lastEmitted);

        void EmitIfChanged(Location? next)
        {
            if (EqualityComparer<Location?>.Default.Equals(lastEmitted, next))
            {
                return;
            }

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
                ex => Console.WriteLine($"[Firebase Error] Location: {ex.Message}")
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
                ex => Console.WriteLine($"[Firebase Error] Location: {ex.Message}")
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
                ex => Console.WriteLine($"[Firebase Error] Location: {ex.Message}")
            );

        var pollingCancellation = new CancellationTokenSource();
        var pollingTask = Task.Run(async () =>
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            while (await timer.WaitForNextTickAsync(pollingCancellation.Token))
            {
                var polled = await locationQuery.OnceSingleAsync<Location?>();
                lastLatitude = polled?.Latitude;
                lastLongitude = polled?.Longitude;
                EmitIfChanged(polled);
            }
        }, pollingCancellation.Token);

        _ = pollingTask.ContinueWith(
            t => Console.WriteLine($"[Firebase Error] Location polling: {t.Exception?.GetBaseException().Message}"),
            TaskContinuationOptions.OnlyOnFaulted
        );
        _ = pollingTask.ContinueWith(_ => pollingCancellation.Dispose(), TaskScheduler.Default);

        var composite = new CompositeSubscription(
            [locationSubscription, latitudeSubscription, longitudeSubscription],
            () =>
            {
                if (!pollingCancellation.IsCancellationRequested)
                {
                    pollingCancellation.Cancel();
                }
            }
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
