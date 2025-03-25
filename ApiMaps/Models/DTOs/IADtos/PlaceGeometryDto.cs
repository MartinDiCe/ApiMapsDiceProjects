using System.Text.Json.Serialization;

namespace ApiMaps.Models.DTOs.IADtos;

/// <summary>
/// Representa la sección "geometry" para un lugar.
/// </summary>
public class PlaceGeometryDto
{
    [JsonPropertyName("location")]
    public PlaceLocationDto? Location { get; set; }
}