using System.Text.Json.Serialization;

namespace ApiMaps.Models.DTOs.GeocodeResponseDtos;

/// <summary>
    /// DTO para cada objeto dentro del array "results".
    /// </summary>
    public class GeocodeResultDto
    {
        /// <summary>
        /// Dirección formateada completa (ej: "Av. Jujuy 489, C1083...").
        /// </summary>
        [JsonPropertyName("formatted_address")]
        public string? FormattedAddress { get; set; }

        /// <summary>
        /// Datos de geometría (lat/lng, viewport, etc.).
        /// </summary>
        [JsonPropertyName("geometry")]
        public GeometryDto? Geometry { get; set; }

        /// <summary>
        /// Indica si la coincidencia es parcial (true) o completa (false).
        /// </summary>
        [JsonPropertyName("partial_match")]
        public bool? PartialMatch { get; set; }

        /// <summary>
        /// Identificador único del lugar, usado para la API Places.
        /// </summary>
        [JsonPropertyName("place_id")]
        public string? PlaceId { get; set; }

        /// <summary>
        /// Lista de tipos para este resultado (street_address, route, locality...).
        /// </summary>
        [JsonPropertyName("types")]
        public List<string>? Types { get; set; }

        /// <summary>
        /// Componentes de la dirección (ej: calle, número, ciudad, etc.).
        /// </summary>
        [JsonPropertyName("address_components")]
        public List<AddressComponentDto>? AddressComponents { get; set; }
    }

    /// <summary>
    /// Representa la sección "geometry" de un "GeocodeResultDto".
    /// Incluye ubicación y viewport, así como el tipo de ubicación.
    /// </summary>
    public class GeometryDto
    {
        [JsonPropertyName("location")]
        public LocationDto? Location { get; set; }

        /// <summary>
        /// Indica la precisión o tipo de la ubicación (ROOFTOP, RANGE_INTERPOLATED, etc.).
        /// </summary>
        [JsonPropertyName("location_type")]
        public string? LocationType { get; set; }

        /// <summary>
        /// Rectángulo que define la vista ideal para mostrar el resultado.
        /// </summary>
        [JsonPropertyName("viewport")]
        public ViewportDto? Viewport { get; set; }
    }

    /// <summary>
    /// Representa "location" dentro de "geometry".
    /// </summary>
    public class LocationDto
    {
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }
        
        [JsonPropertyName("lng")]
        public double Longitude { get; set; }
    }

    /// <summary>
    /// Representa la sección "viewport" dentro de "geometry".
    /// </summary>
    public class ViewportDto
    {
        [JsonPropertyName("northeast")]
        public LocationDto? Northeast { get; set; }

        [JsonPropertyName("southwest")]
        public LocationDto? Southwest { get; set; }
    }

    /// <summary>
    /// Representa cada objeto dentro de "address_components".
    /// </summary>
    public class AddressComponentDto
    {
        [JsonPropertyName("long_name")] public string? LongName { get; set; }

        [JsonPropertyName("short_name")] public string? ShortName { get; set; }

        [JsonPropertyName("types")] public List<string>? Types { get; set; }
    }