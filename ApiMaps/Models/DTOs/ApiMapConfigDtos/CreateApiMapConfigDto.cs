using System.ComponentModel.DataAnnotations;

namespace ApiMaps.Models.DTOs.ApiMapConfigDtos;

/// <summary>
/// DTO para crear una nueva configuración de proveedor de mapas.
/// </summary>
public class CreateApiMapConfigDto
{
    /// <summary>
    /// Nombre del proveedor de mapas (por ejemplo, "GoogleMaps", "Waze").
    /// </summary>
    [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre del proveedor no debe exceder 100 caracteres.")]
    public string Proveedor { get; set; } = string.Empty;

    /// <summary>
    /// URL del endpoint del proveedor de mapas.
    /// </summary>
    [Required(ErrorMessage = "El endpoint es obligatorio.")]
    [Url(ErrorMessage = "El valor proporcionado no es una URL válida.")]
    public string EndPoint { get; set; } = string.Empty;

    /// <summary>
    /// Clave de API para autenticar las solicitudes al proveedor de mapas.
    /// </summary>
    [Required(ErrorMessage = "La API key es obligatoria.")]
    public string ApiKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Parámetros de endpoint del proveedor, en caso de ser necesarios.
    /// </summary>
    public string? EndPointParameter { get; set; } = string.Empty;

    /// <summary>
    /// Parámetros adicionales para la configuración, en caso de ser necesarios.
    /// </summary>
    public string? Additional { get; set; }

    /// <summary>
    /// Prioridad del proveedor de mapas. Un valor menor indica mayor preferencia.
    /// </summary>
    [Required(ErrorMessage = "La prioridad es obligatoria.")]
    public int Prioridad { get; set; }
}
