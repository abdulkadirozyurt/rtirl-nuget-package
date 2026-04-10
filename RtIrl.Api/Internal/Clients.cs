using Firebase.Database;
using Firebase.Database.Query;
using RtIrl.Api.Abstractions;
using RtIrl.Api.Models;
using System.Reactive.Linq;

namespace RtIrl.Api.Internal;

internal class PullKeyClient : IPullKeyClient
{
    private readonly FirebaseClient _client;
    private readonly string _pullKey;
    private readonly List<IDisposable> _subscriptions = new();

    public PullKeyClient(FirebaseClient client, string pullKey)
    {
        _client = client;
        _pullKey = pullKey;
    }

    public IDisposable AddLocationListener(Action<Location> callback) => Subscribe("location", callback);
    public IDisposable AddSpeedListener(Action<double> callback) => Subscribe("speed", callback);
    public IDisposable AddHeadingListener(Action<double> callback) => Subscribe("heading", callback);
    public IDisposable AddAltitudeListener(Action<double> callback) => Subscribe("altitude", callback);
    public IDisposable AddHeartRateListener(Action<int> callback) => Subscribe("heartRate", callback);
    public IDisposable AddCyclingPowerListener(Action<int> callback) => Subscribe("cyclingPower", callback);
    public IDisposable AddPedometerStepsListener(Action<long> callback) => Subscribe("pedometerSteps", callback);
    public IDisposable AddSessionIdListener(Action<string?> callback) => Subscribe("sessionId", callback);
    public IDisposable AddListener(Action<object?> callback) => Subscribe("", callback);

    private IDisposable Subscribe<T>(string property, Action<T> callback)
    {
        var query = _client.Child("pullables").Child(_pullKey);
        if (!string.IsNullOrEmpty(property))
        {
            query = query.Child(property);
        }

        // JS'deki onValue mantığı: FirebaseEvent akışını dinle
        // Bu yapı hem ilk değeri hem de sonraki tüm değişimleri yakalar.
        var subscription = query
            .AsObservable<T>()
            .Subscribe(d => {
                if (d.Object != null)
                {
                    callback(d.Object);
                }
            }, ex => Console.WriteLine($"[Firebase Error] {property}: {ex.Message}"));

        _subscriptions.Add(subscription);
        return subscription;
    }

    public void Dispose()
    {
        foreach (var sub in _subscriptions) sub.Dispose();
        _subscriptions.Clear();
    }
}

internal class StreamerClient : IStreamerClient
{
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
        // Doğrudan location düğümünü izle (JS SDK ile aynı mantık)
        var subscription = _client
            .Child("streamers")
            .Child(_path)
            .Child("location")
            .AsObservable<Location?>()
            .Subscribe(d =>
            {
                // EventType: Put veya Patch olduğunda tetiklenir
                callback(d.Object);
            }, ex => Console.WriteLine($"[Firebase Error] Location: {ex.Message}"));

        _subscriptions.Add(subscription);
        return subscription;
    }

    public void Dispose()
    {
        foreach (var sub in _subscriptions) sub.Dispose();
        _subscriptions.Clear();
    }
}
