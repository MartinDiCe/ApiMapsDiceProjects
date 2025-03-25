using ApiMaps.Models.DTOs.IADtos;

namespace ApiMaps.Models.DTOs.GeocodeResponseDtos

{
    /// <summary>
    /// DTO que representa la respuesta final del proceso completo (IA + Geocode + Places).
    /// </summary>
    public class RefinedGeocodeResponseDto
    {
        /// <summary>
        /// Dirección ingresada originalmente por el usuario.
        /// </summary>
        public string OriginalAddress { get; set; } = string.Empty;

        /// <summary>
        /// Dirección refinada por la IA (si existía configuración).
        /// Si no se utilizó IA, coincide con la original.
        /// </summary>
        public string RefinedAddress { get; set; } = string.Empty;

        /// <summary>
        /// Colección de respuestas de geocodificación (p. ej. “all” providers).
        /// </summary>
        public IList<GeocodeResponseDto> GeocodeResults { get; set; } = new List<GeocodeResponseDto>();

        /// <summary>
        /// Lat/Lng extraídos del primer resultado válido (si lo hubo).
        /// </summary>
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        /// <summary>
        /// Lista de lugares cercanos devueltos por Google Places (si configurado).
        /// </summary>
        public PlacesResultDto? NearbyPlaces { get; set; }

        /// <summary>
        /// Radio usado para Places (si > 0).
        /// </summary>
        public int? UsedRadius { get; set; }

        /// <summary>
        /// Logs que describen cada paso (realizado o saltado, con sus motivos).
        /// </summary>
        public List<string> ProcessLogs { get; set; } = new List<string>();
    }
}
