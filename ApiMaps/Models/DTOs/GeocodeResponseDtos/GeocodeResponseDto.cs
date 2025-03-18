using System.Text.Json.Serialization;

namespace ApiMaps.Models.DTOs.GeocodeResponseDtos
{
    /// <summary>
    /// DTO que representa la respuesta principal de la API de geocodificación de Google.
    /// </summary>
    public class GeocodeResponseDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("results")]
        public List<GeocodeResultDto> Results { get; set; } = new();
    }
}
