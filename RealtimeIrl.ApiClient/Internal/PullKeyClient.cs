using Firebase.Database;
using Firebase.Database.Query;
using RealtimeIrl.ApiClient.Abstractions;
using RealtimeIrl.ApiClient.Models;
using System.Reactive.Linq;

namespace RealtimeIrl.ApiClient.Internal;

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
    public IDisposable AddCyclingCrankListener(Action<int> callback) => Subscribe("cyclingCrank", callback);
    public IDisposable AddCyclingWheelListener(Action<int> callback) => Subscribe("cyclingWheel", callback);
    public IDisposable AddPedometerStepsListener(Action<long> callback) => Subscribe("pedometerSteps", callback);
    public IDisposable AddSessionIdListener(Action<string?> callback) => SubscribeNullable("sessionId", callback);
    public IDisposable AddListener(Action<object?> callback) => SubscribeNullable("", callback);

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
            .Subscribe(d =>
            {
                if (d.Object != null)
                {
                    callback(d.Object);
                }
            }, ex => Console.WriteLine($"[Firebase Error] {property}: {ex.Message}"));

        _subscriptions.Add(subscription);
        return subscription;
    }

    private IDisposable SubscribeNullable<T>(string property, Action<T?> callback) where T : class
    {
        var query = _client.Child("pullables").Child(_pullKey);
        if (!string.IsNullOrEmpty(property))
        {
            query = query.Child(property);
        }

        var subscription = query
            .AsObservable<T?>()
            .Subscribe(
                d => callback(d.Object),
                ex => Console.WriteLine($"[Firebase Error] {property}: {ex.Message}")
            );

        _subscriptions.Add(subscription);
        return subscription;
    }

    public void Dispose()
    {
        foreach (var sub in _subscriptions) sub.Dispose();
        _subscriptions.Clear();
    }
}
