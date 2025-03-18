using ApiMaps.Models.Entities;

namespace ApiMaps.Models.Repositories.ApiMapConfigRepositories;

/// <summary>
/// Define las operaciones CRUD para la entidad <see cref="ApiMapConfig"/>.
/// Este repositorio se comunica únicamente con la base de datos.
/// </summary>
public interface IApiMapConfigRepository
{
    /// <summary>
    /// Obtiene la configuración de un proveedor de mapas por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador único de la configuración.</param>
    /// <returns>La configuración encontrada o null si no existe.</returns>
    Task<ApiMapConfig?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene todas las configuraciones de proveedores de mapas de forma asíncrona.
    /// </summary>
    /// <returns>Una lista de configuraciones de proveedores de mapas.</returns>
    Task<IEnumerable<ApiMapConfig>> GetAllAsync();

    /// <summary>
    /// Agrega una nueva configuración de proveedor de mapas de forma asíncrona.
    /// </summary>
    /// <param name="entity">La entidad <see cref="ApiMapConfig"/> a agregar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task AddAsync(ApiMapConfig entity);

    /// <summary>
    /// Actualiza una configuración de proveedor de mapas existente de forma asíncrona.
    /// </summary>
    /// <param name="entity">La entidad <see cref="ApiMapConfig"/> a actualizar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task UpdateAsync(ApiMapConfig entity);

    /// <summary>
    /// Elimina una configuración de proveedor de mapas por su identificador de forma asíncrona.
    /// </summary>
    /// <param name="id">Identificador de la configuración a eliminar.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task DeleteAsync(int id);
}
