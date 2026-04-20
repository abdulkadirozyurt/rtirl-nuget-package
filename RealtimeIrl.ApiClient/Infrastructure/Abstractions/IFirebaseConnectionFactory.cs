using Firebase.Database;

namespace RealtimeIrl.ApiClient.Infrastructure.Abstractions;

public interface IFirebaseConnectionFactory
{
    FirebaseClient CreateClient();
}
