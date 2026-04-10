namespace RtIrl.Api.Models;

/// <summary>
/// Represents the Firebase application configuration, similar to the JS SDK.
/// </summary>
public record FirebaseAppConfig
{
    public string ApiKey { get; init; } = "AIzaSyC4L8ICZbJDufxe8bimRdB5cAulPCaYVQQ";
    public string DatabaseUrl { get; init; } = "https://rtirl-a1d7f-default-rtdb.firebaseio.com";
    public string ProjectId { get; init; } = "rtirl-a1d7f";
    public string AppId { get; init; } = "1:684852107701:web:d77a8ed0ee5095279a61fc";
    public string MeasurementId { get; init; } = "G-TR97D81LT3";
}
