namespace ApiMaps.Models.DTOs.GeocodeResponseDtos;

/// <summary>
/// Representa la respuesta de geocodificación de un proveedor.
/// </summary>
public class GeocodeResponseDto
{
    /// <summary>
    /// Estado de la respuesta (por ejemplo, "OK").
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Lista de resultados obtenidos.
    /// </summary>
    public IEnumerable<GeocodeResultDto> Results { get; set; }
    
}

