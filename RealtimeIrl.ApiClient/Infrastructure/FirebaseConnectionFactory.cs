using Firebase.Database;
using RealtimeIrl.ApiClient.Infrastructure.Abstractions;

namespace RealtimeIrl.ApiClient.Infrastructure;

internal class FirebaseConnectionFactory(string databaseUrl) : IFirebaseConnectionFactory
{
    private readonly string _databaseUrl = string.IsNullOrWhiteSpace(databaseUrl)
        ? throw new ArgumentException("Firebase database URL cannot be null or empty.", nameof(databaseUrl))
        : databaseUrl;

    public FirebaseClient CreateClient()
    {
        return new FirebaseClient(_databaseUrl);
    }
}
