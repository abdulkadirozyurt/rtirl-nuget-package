using Firebase.Database;
using RtIrl.Api.Models;

namespace RtIrl.Api.Infrastructure;

public interface IFirebaseConnectionFactory
{
    FirebaseClient CreateClient();
}

public class RtIrlFirebaseConnectionFactory : IFirebaseConnectionFactory
{
    // Npm paketindeki orijinal Firebase konfigürasyonu
    private const string BaseUrl = "https://rtirl-a1d7f-default-rtdb.firebaseio.com";

    public FirebaseClient CreateClient()
    {
        // WebSocket desteğini en sağlam şekilde kurmak için 
        // varsayılan seçenekleri kullanalım.
        return new FirebaseClient(BaseUrl);
    }
}
