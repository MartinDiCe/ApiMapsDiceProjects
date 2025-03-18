using ApiMaps.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMaps.Models.Entities
{
    /// <summary>
    /// Representa la configuración de un proveedor de mapas, incluyendo los detalles necesarios para acceder a sus servicios.
    /// Hereda de <see cref="AuditEntities"/> para incluir propiedades de auditoría (fecha de creación, modificación, etc.).
    /// </summary>
    [Table("ApiMapConfigs")]
    public class ApiMapConfig : AuditEntities
    {
        /// <summary>
        /// Identificador único de la configuración.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nombre del proveedor de mapas (por ejemplo, "GoogleMaps", "Waze").
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Proveedor { get; set; } = string.Empty;

        /// <summary>
        /// URL del endpoint del proveedor de mapas.
        /// Debe ser una URL válida, de lo contrario se mostrará un mensaje de error.
        /// </summary>
        [Required]
        [Url(ErrorMessage = "El valor proporcionado no es una URL válida.")]
        public string EndPoint { get; set; } = string.Empty;

        /// <summary>
        /// Clave de API necesaria para autenticar las solicitudes al proveedor de mapas.
        /// </summary>
        [Required]
        public string ApiKey { get; set; } = string.Empty;
        
        /// <summary>
        /// Parámetros de endpoint del proveedor, en caso de ser necesarios.
        /// </summary>
        public string? EndPointParameter { get; set; } = string.Empty;
        
        /// <summary>
        /// Parámetros adicionales para la configuración o personalización del proveedor, en caso de ser necesarios.
        /// </summary>
        public string? Additional { get; set; }
        
        /// <summary>
        /// Prioridad del proveedor de mapas.
        /// Se utiliza para determinar el orden de preferencia cuando se tienen múltiples proveedores.
        /// Un valor menor indica una mayor prioridad.
        /// </summary>
        [Required]
        public int Prioridad { get; set; }
        
    }
}