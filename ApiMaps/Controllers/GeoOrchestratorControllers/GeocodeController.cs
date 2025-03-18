using System.Reactive.Linq;
using ApiMaps.Services.GeocodingServices;
using Microsoft.AspNetCore.Mvc;

namespace ApiMaps.Controllers.GeoOrchestratorControllers
{
    /// <summary>
    /// Controlador para gestionar operaciones de geocodificación.
    /// </summary>
    [ApiController]
    [Route("api/geocode")]
    public class GeocodeController : ControllerBase
    {
        private readonly IGeocodeService _geocodeService;

        /// <summary>
        /// Constructor que inyecta el servicio central de geocodificación.
        /// </summary>
        /// <param name="geocodeService">Servicio central que orquesta la geocodificación.</param>
        public GeocodeController(IGeocodeService geocodeService)
        {
            _geocodeService = geocodeService ?? throw new ArgumentNullException(nameof(geocodeService));
        }

        /// <summary>
        /// Procesa una dirección y devuelve la respuesta de geocodificación.
        /// </summary>
        /// <param name="address">La dirección a geocodificar.</param>
        /// <param name="mode">
        /// Modo de consulta:
        /// - "first": Retorna el primer resultado exitoso.
        /// - "all": Retorna los resultados de todos los proveedores.
        /// - "group": Retorna un diccionario agrupado por proveedor basado en las prioridades.
        /// Por defecto se usa "first".
        /// </param>
        /// <param name="priorities">
        /// (Opcional, para el modo "group") Una cadena con las prioridades a considerar, separadas por comas (por ejemplo, "1,2").
        /// Si no se especifica, se utiliza la prioridad 1 por defecto.
        /// </param>
        /// <returns>Respuesta de geocodificación.</returns>
        [HttpGet("process")]
        public async Task<IActionResult> ProcessAddress(
            [FromQuery] string address,
            [FromQuery] string mode = "first",
            [FromQuery] string priorities = null)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return BadRequest("La dirección es obligatoria.");
            }

            try
            {
                if (mode.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    var allResults = await _geocodeService.ProcessAddressAllAsync(address).FirstAsync();
                    return Ok(allResults);
                }
                else if (mode.Equals("group", StringComparison.OrdinalIgnoreCase))
                {
                    var priorityList = string.IsNullOrWhiteSpace(priorities)
                        ? new List<int> { 1 }
                        : priorities.Split(',')
                            .Select(s => int.TryParse(s, out int p) ? p : (int?)null)
                            .Where(p => p.HasValue)
                            .Select(p => p.Value)
                            .ToList();

                    var groupedResults = await _geocodeService.ProcessAddressGroupedAsync(address, priorityList).FirstAsync();
                    return Ok(groupedResults);
                }
                else
                {
                    var result = await _geocodeService.ProcessAddressAsync(address).FirstAsync();
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message, detalle = ex.InnerException?.Message });
            }
        }
    }
}