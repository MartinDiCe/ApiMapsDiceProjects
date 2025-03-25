using System.Text.Json.Serialization;

namespace ApiMaps.Models.DTOs.IADtos;

/// <summary>
/// Representa la respuesta general de la API de Google Places
/// para la búsqueda de lugares cercanos (Nearby Search).
/// </summary>
public class PlacesResultDto
{
    /// <summary>
    /// Estado de la solicitud (ej: "OK", "ZERO_RESULTS", etc.).
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Lista de lugares encontrados.
    /// </summary>
    [JsonPropertyName("results")]
    public List<PlaceItemDto> Results { get; set; } = new();

    /// <summary>
    /// Información adicional de atribuciones, si es que la API la provee.
    /// </summary>
    [JsonPropertyName("html_attributions")]
    public List<string> HtmlAttributions { get; set; } = new();
}