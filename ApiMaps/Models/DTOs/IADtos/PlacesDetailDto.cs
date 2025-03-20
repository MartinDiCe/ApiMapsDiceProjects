using System.Text.Json.Serialization;

namespace ApiMaps.Models.DTOs.IADtos;

/// <summary>
/// Respuesta de la API de Google Places para "place/details".
/// </summary>
public class PlacesDetailDto
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("result")]
    public PlaceDetailResultDto? Result { get; set; }

    [JsonPropertyName("html_attributions")]
    public List<string> HtmlAttributions { get; set; } = new();
}