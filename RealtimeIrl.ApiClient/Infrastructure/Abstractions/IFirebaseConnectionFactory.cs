using Firebase.Database;

namespace RealtimeIrl.ApiClient.Infrastructure.Abstractions;

internal interface IFirebaseConnectionFactory
{
    FirebaseClient CreateClient();
}
