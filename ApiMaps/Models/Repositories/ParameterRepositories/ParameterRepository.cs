using ApiMaps.Data;
using ApiMaps.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiMaps.Models.Repositories.ParameterRepositories
{
    /// <summary>
    /// Implementa el repositorio para la entidad <see cref="Parameter"/> utilizando Entity Framework Core.
    /// Se encarga únicamente de la comunicación con la base de datos, siguiendo principios SOLID y manejo adecuado de errores.
    /// </summary>
    public class ParameterRepository : IParameterRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ParameterRepository> _logger;

        /// <summary>
        /// Crea una nueva instancia de <see cref="ParameterRepository"/>.
        /// </summary>
        /// <param name="context">El contexto de datos de la aplicación.</param>
        /// <param name="logger">El logger para registrar eventos y errores.</param>
        public ParameterRepository(ApplicationDbContext context, ILogger<ParameterRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<Parameter> CreateAsync(Parameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            try
            {
                await _context.Parameters.AddAsync(parameter);
                await _context.SaveChangesAsync();
                return parameter;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el parámetro {ParameterName}", parameter.ParameterName);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Parameter?> GetByIdAsync(int parameterId)
        {
            try
            {
                return await _context.Parameters.FirstOrDefaultAsync(p => p.ParameterId == parameterId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el parámetro con Id {ParameterId}", parameterId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Parameter?> GetByNameAsync(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException("El nombre del parámetro es requerido.", nameof(parameterName));
            }

            try
            {
                return await _context.Parameters.FirstOrDefaultAsync(
                    p => p.ParameterName.ToLower() == parameterName.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el parámetro con nombre {ParameterName}", parameterName);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Parameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            try
            {
                _context.Parameters.Update(parameter);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el parámetro con Id {ParameterId}", parameter.ParameterId);
                throw;
            }
        }
    }
}