using System.Text.Json.Serialization;

namespace ApiMaps.Models.DTOs.IADtos;

/// <summary>
/// Contiene la info detallada de un lugar específico.
/// </summary>
public class PlaceDetailResultDto
{
    [JsonPropertyName("place_id")]
    public string? PlaceId { get; set; }

    /// <summary>
    /// Nombre del lugar (ej: "Café Martinez").
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("geometry")]
    public PlaceGeometryDto? Geometry { get; set; }

    /// <summary>
    /// Dirección formateada completa.
    /// </summary>
    [JsonPropertyName("formatted_address")]
    public string? FormattedAddress { get; set; }

    /// <summary>
    /// Lista de tipos que describen al lugar.
    /// </summary>
    [JsonPropertyName("types")]
    public List<string>? Types { get; set; }

    /// <summary>
    /// Teléfono internacional (si está disponible).
    /// </summary>
    [JsonPropertyName("international_phone_number")]
    public string? InternationalPhoneNumber { get; set; }

    /// <summary>
    /// Horarios de apertura/cierre (si están disponibles).
    /// </summary>
    [JsonPropertyName("opening_hours")]
    public OpeningHoursDto? OpeningHours { get; set; }

    // Podés agregar muchas más propiedades (rating, photos, reviews, etc.)
}
