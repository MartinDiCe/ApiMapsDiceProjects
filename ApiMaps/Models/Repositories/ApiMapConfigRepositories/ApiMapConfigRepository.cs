using ApiMaps.Data;
using ApiMaps.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiMaps.Models.Repositories.ApiMapConfigRepositories;

/// <summary>
/// Implementa el repositorio para la entidad <see cref="ApiMapConfig"/> utilizando Entity Framework Core.
/// Se encarga únicamente de la comunicación con la base de datos.
/// </summary>
public class ApiMapConfigRepository : IApiMapConfigRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApiMapConfigRepository> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="ApiMapConfigRepository"/>.
    /// </summary>
    /// <param name="context">El contexto de la base de datos.</param>
    /// <param name="logger">El logger para registrar eventos y errores.</param>
    public ApiMapConfigRepository(ApplicationDbContext context, ILogger<ApiMapConfigRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ApiMapConfig?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.ApiMapConfigs.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ApiMapConfig con Id {Id}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ApiMapConfig>> GetAllAsync()
    {
        try
        {
            return await _context.ApiMapConfigs.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las configuraciones de ApiMapConfigs");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task AddAsync(ApiMapConfig entity)
    {
        try
        {
            await _context.ApiMapConfigs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar la configuración de ApiMapConfig para el servicio {Proveedor}",
                entity.Proveedor);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(ApiMapConfig entity)
    {
        try
        {
            _context.ApiMapConfigs.Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar ApiMapConfig con Id {Id}", entity.Id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await _context.ApiMapConfigs.FindAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("No se encontró ApiMapConfig con Id {Id} para eliminar", id);
                return;
            }

            _context.ApiMapConfigs.Remove(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar ApiMapConfig con Id {Id}", id);
            throw;
        }
    }
}
