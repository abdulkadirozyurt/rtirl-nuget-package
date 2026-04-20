using Firebase.Database;
using RealtimeIrl.ApiClient.Infrastructure.Abstractions;

namespace RealtimeIrl.ApiClient.Infrastructure;

public class FirebaseConnectionFactory(string databaseUrl) : IFirebaseConnectionFactory
{
    private readonly string _databaseUrl = databaseUrl;

    public FirebaseClient CreateClient()
    {
        return new FirebaseClient(_databaseUrl);
    }
}
