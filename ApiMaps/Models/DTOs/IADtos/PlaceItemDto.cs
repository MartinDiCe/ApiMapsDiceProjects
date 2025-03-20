using System.Text.Json.Serialization;

namespace ApiMaps.Models.DTOs.IADtos;

/// <summary>
/// Representa cada lugar dentro de 'results'.
/// </summary>
public class PlaceItemDto
{
    [JsonPropertyName("place_id")]
    public string? PlaceId { get; set; }

    /// <summary>
    /// Nombre del lugar (ej: "Café Martinez").
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Información de geometría, lat/lng.
    /// </summary>
    [JsonPropertyName("geometry")]
    public PlaceGeometryDto? Geometry { get; set; }

    /// <summary>
    /// Listado de categorías o tipos de lugar (ej: "cafe", "restaurant").
    /// </summary>
    [JsonPropertyName("types")]
    public List<string>? Types { get; set; }

    /// <summary>
    /// Dirección aproximada (ej: "Av. Jujuy 500...").
    /// </summary>
    [JsonPropertyName("vicinity")]
    public string? Vicinity { get; set; }

    }