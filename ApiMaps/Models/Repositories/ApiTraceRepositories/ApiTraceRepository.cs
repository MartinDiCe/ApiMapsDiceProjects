using ApiMaps.Data;
using ApiMaps.Models.Entities;

namespace ApiMaps.Models.Repositories.ApiTraceRepositories
{
    /// <summary>
    /// Implementa el repositorio para el registro de trazas de la API utilizando Entity Framework Core.
    /// </summary>
    public class ApiTraceRepository(ApplicationDbContext context) : IApiTraceRepository
    {
        private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        /// <inheritdoc />
        public async Task<ApiTrace> CreateAsync(ApiTrace trace)
        {
            if (trace == null)
            {
                throw new ArgumentNullException(nameof(trace));
            }

            _context.ApiTraces.Add(trace);
            await _context.SaveChangesAsync();
            return trace;
        }
    }
}