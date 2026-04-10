using Newtonsoft.Json;

namespace RtIrl.Api.Models;

/// <summary>
/// Represents a geographical location with latitude and longitude.
/// </summary>
public record Location(
    [property: JsonProperty("latitude")] double Latitude, 
    [property: JsonProperty("longitude")] double Longitude);
