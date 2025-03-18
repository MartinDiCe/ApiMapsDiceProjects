using ApiMaps.Models.Entities;
using System.Reactive;
using ApiMaps.Models.DTOs.ApiMapConfigDtos;

namespace ApiMaps.Services.ApiMapConfigServices
{
    /// <summary>
    /// Define las operaciones del servicio para gestionar la configuración de proveedores de mapas (ApiMapConfig)
    /// de forma 100% reactiva.
    /// </summary>
    public interface IApiMapConfigService
    {
        /// <summary>
        /// Obtiene la configuración de un proveedor de mapas por su identificador de forma reactiva.
        /// </summary>
        /// <param name="id">Identificador único de la configuración.</param>
        /// <returns>
        /// Un observable que emite la configuración encontrada o null si no existe.
        /// </returns>
        IObservable<ApiMapConfig?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene todas las configuraciones de proveedores de mapas de forma reactiva.
        /// </summary>
        /// <returns>
        /// Un observable que emite una colección de configuraciones.
        /// </returns>
        IObservable<IEnumerable<ApiMapConfig>> GetAllAsync();

        /// <summary>
        /// Crea una nueva configuración para un proveedor de mapas, aplicando la lógica de negocio necesaria.
        /// </summary>
        /// <param name="dto">
        /// Datos necesarios para crear la configuración.
        /// </param>
        /// <returns>
        /// Un observable que emite la configuración creada.
        /// </returns>
        IObservable<ApiMapConfig> CreateAsync(CreateApiMapConfigDto dto);

        /// <summary>
        /// Actualiza una configuración existente de un proveedor de mapas.
        /// </summary>
        /// <param name="dto">
        /// Datos necesarios para actualizar la configuración.
        /// </param>
        /// <returns>
        /// Un observable que emite la configuración actualizada.
        /// </returns>
        IObservable<ApiMapConfig> UpdateAsync(UpdateApiMapConfigDto dto);

        /// <summary>
        /// Elimina una configuración de un proveedor de mapas por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la configuración a eliminar.</param>
        /// <returns>
        /// Un observable que emite <see cref="Unit"/> cuando la eliminación es exitosa.
        /// </returns>
        IObservable<Unit> DeleteAsync(int id);
    }
}
