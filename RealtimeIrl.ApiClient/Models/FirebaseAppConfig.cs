namespace RealtimeIrl.ApiClient.Models;

/// <summary>
/// Represents the Firebase application configuration, similar to the JS SDK.
/// </summary>
public record FirebaseAppConfig
{
    /// <summary>
    /// Firebase Realtime Database URL used by RealtimeIRL.
    /// </summary>
    public string DatabaseUrl { get; init; } = "https://rtirl-a1d7f-default-rtdb.firebaseio.com";
}
