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
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _pullKey = string.IsNullOrWhiteSpace(pullKey)
            ? throw new ArgumentException("Pull key cannot be null or empty.", nameof(pullKey))
            : pullKey;
    }

    public IDisposable AddLocationListener(Action<Location> callback, Action<Exception>? onError = null) => Subscribe("location", callback, onError);
    public IDisposable AddSpeedListener(Action<double> callback, Action<Exception>? onError = null) => Subscribe("speed", callback, onError);
    public IDisposable AddHeadingListener(Action<double> callback, Action<Exception>? onError = null) => Subscribe("heading", callback, onError);
    public IDisposable AddAltitudeListener(Action<double> callback, Action<Exception>? onError = null) => Subscribe("altitude", callback, onError);
    public IDisposable AddHeartRateListener(Action<int> callback, Action<Exception>? onError = null) => Subscribe("heartRate", callback, onError);
    public IDisposable AddCyclingPowerListener(Action<int> callback, Action<Exception>? onError = null) => Subscribe("cyclingPower", callback, onError);
    public IDisposable AddCyclingCrankListener(Action<int> callback, Action<Exception>? onError = null) => Subscribe("cyclingCrank", callback, onError);
    public IDisposable AddCyclingWheelListener(Action<int> callback, Action<Exception>? onError = null) => Subscribe("cyclingWheel", callback, onError);
    public IDisposable AddPedometerStepsListener(Action<long> callback, Action<Exception>? onError = null) => Subscribe("pedometerSteps", callback, onError);
    public IDisposable AddSessionIdListener(Action<string?> callback, Action<Exception>? onError = null) => SubscribeNullable("sessionId", callback, onError);
    public IDisposable AddListener(Action<object?> callback, Action<Exception>? onError = null) => SubscribeNullable("", callback, onError);

    private IDisposable Subscribe<T>(string property, Action<T> callback, Action<Exception>? onError)
    {
        ArgumentNullException.ThrowIfNull(callback);

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
            }, ex => onError?.Invoke(ex));

        _subscriptions.Add(subscription);
        return subscription;
    }

    private IDisposable SubscribeNullable<T>(string property, Action<T?> callback, Action<Exception>? onError) where T : class
    {
        ArgumentNullException.ThrowIfNull(callback);

        var query = _client.Child("pullables").Child(_pullKey);
        if (!string.IsNullOrEmpty(property))
        {
            query = query.Child(property);
        }

        var subscription = query
            .AsObservable<T?>()
            .Subscribe(
                d => callback(d.Object),
                ex => onError?.Invoke(ex)
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
