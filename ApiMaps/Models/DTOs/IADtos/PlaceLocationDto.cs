using System.Text.Json.Serialization;

namespace ApiMaps.Models.DTOs.IADtos;

/// <summary>
/// Coordenadas lat/lng.
/// </summary>
public class PlaceLocationDto
{
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lng")]
    public double Longitude { get; set; }
}