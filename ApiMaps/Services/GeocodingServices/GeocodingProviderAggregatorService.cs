using System.Reactive.Linq;
using ApiMaps.Models.DTOs.GeocodeResponseDtos;
using ApiMaps.Services.LoggingServices;

namespace ApiMaps.Services.GeocodingServices
{
    /// <summary>
    /// Agrega múltiples proveedores de geocodificación y permite obtener:
    /// - Un único resultado de los proveedores con la prioridad más alta.
    /// - Todos los resultados de todos los proveedores.
    /// - Los resultados agrupados por proveedor, filtrando por un conjunto de prioridades.
    /// </summary>
    /// <remarks>
    /// La lógica de filtrado se basa en el campo <c>Priority</c> de cada proveedor.
    /// Se registra el proceso de selección, la invocación a cada proveedor y la agrupación de resultados.
    /// </remarks>
    public class GeocodingProviderAggregatorService : IGeocodingProviderAggregatorService
    {
        // Se materializa la colección de proveedores en el constructor para evitar múltiples enumeraciones.
        private readonly IList<IGeocodingProvider> _providers;
        private readonly ILoggerService<GeocodingProviderAggregatorService> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="GeocodingProviderAggregatorService"/>.
        /// </summary>
        /// <param name="providers">Colección de proveedores individuales de geocodificación.</param>
        /// <param name="logger">Servicio de logging para el agregador.</param>
        public GeocodingProviderAggregatorService(
            IEnumerable<IGeocodingProvider> providers,
            ILoggerService<GeocodingProviderAggregatorService> logger)
        {
            _providers = providers?.ToList() ?? throw new ArgumentNullException(nameof(providers));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public string ProviderName => "Aggregator";

        /// <inheritdoc/>
        public int Priority => _providers.Min(p => p.Priority);

        /// <inheritdoc/>
        public IObservable<GeocodeResponseDto> GeocodeAddressAsync(string address)
        {
            _logger.LogInfo($"[Aggregator] Procesando dirección: {address}");

            // Selecciona los proveedores con la prioridad más alta (valor numérico mínimo).
            int minPriority = _providers.Min(p => p.Priority);
            var selectedProviders = _providers.Where(p => p.Priority == minPriority).ToList();
            _logger.LogInfo($"[Aggregator] Se seleccionaron {selectedProviders.Count} proveedor(es) con prioridad {minPriority}");

            // Para cada proveedor seleccionado, se llama a su método de geocodificación y se captura cualquier error.
            var observables = selectedProviders.Select(provider =>
            {
                _logger.LogInfo($"[Aggregator] Llamando a: {provider.ProviderName} (Priority: {provider.Priority})");
                return provider.GeocodeAddressAsync(address)
                    .Catch<GeocodeResponseDto, Exception>(ex =>
                    {
                        _logger.LogError($"[Aggregator] Error en {provider.ProviderName}: {ex.Message}");
                        return Observable.Empty<GeocodeResponseDto>();
                    });
            });

            // Se combinan (merge) todas las secuencias; si no se produce ningún elemento se lanza un error controlado.
            return observables.Merge()
                .DefaultIfEmpty(null)
                .Select(result =>
                {
                    if (result == null)
                        throw new InvalidOperationException("No se encontraron resultados para la dirección proporcionada.");
                    return result;
                });
        }

        /// <inheritdoc/>
        public IObservable<IList<GeocodeResponseDto>> GeocodeAddressAllAsync(string address)
        {
            _logger.LogInfo($"[Aggregator] Procesando dirección para todos los proveedores: {address}");
            var observables = _providers.Select(provider =>
            {
                _logger.LogInfo($"[Aggregator] Llamando a: {provider.ProviderName} (Priority: {provider.Priority})");
                return provider.GeocodeAddressAsync(address)
                    .Catch<GeocodeResponseDto, Exception>(ex =>
                    {
                        _logger.LogError($"[Aggregator] Error en {provider.ProviderName}: {ex.Message}");
                        return Observable.Empty<GeocodeResponseDto>();
                    });
            });

            return observables.Merge().ToList();
        }

        /// <inheritdoc/>
        public IObservable<IDictionary<string, IList<GeocodeResponseDto>>> GeocodeAddressGroupedAsync(
            string address,
            IEnumerable<int> priorities)
        {
            // Materializamos la lista de prioridades para evitar múltiples enumeraciones.
            var priorityList = priorities.ToList();
            _logger.LogInfo($"[Aggregator] Procesando dirección para proveedores con prioridades: {string.Join(",", priorityList)}");

            var selectedProviders = _providers.Where(p => priorityList.Contains(p.Priority)).ToList();
            _logger.LogInfo($"[Aggregator] Se seleccionaron {selectedProviders.Count} proveedor(es) de las prioridades indicadas.");

            // Se ejecuta la llamada para cada proveedor seleccionado y se crea un objeto anónimo con el nombre y resultado.
            var observables = selectedProviders.Select(provider =>
                provider.GeocodeAddressAsync(address)
                    .Catch<GeocodeResponseDto, Exception>(ex =>
                    {
                        _logger.LogError($"[Aggregator] Error en {provider.ProviderName}: {ex.Message}");
                        return Observable.Empty<GeocodeResponseDto>();
                    })
                    .Select(result => new { provider.ProviderName, Result = result })
            );

            // Se combinan todas las respuestas, se materializan en lista y se agrupan por proveedor.
            return observables.Merge()
                .ToList()
                .Select(results =>
                {
                    IDictionary<string, IList<GeocodeResponseDto>> dict = results
                        .GroupBy(x => x.ProviderName)
                        .ToDictionary(
                            g => g.Key,
                            g => (IList<GeocodeResponseDto>)g.Select(x => x.Result).ToList()
                        );
                    return dict;
                });
        }

        /// <inheritdoc/>
        public IObservable<GeocodeResponseDto> GeocodeAddressByProviderAsync(string address, string providerName)
        {
            _logger.LogInfo($"[Aggregator] Buscando proveedor: {providerName} para la dirección: {address}");
            var provider = _providers.FirstOrDefault(p =>
                p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
            {
                _logger.LogError($"[Aggregator] Proveedor {providerName} no encontrado.");
                return Observable.Throw<GeocodeResponseDto>(new ArgumentException($"Proveedor {providerName} no encontrado."));
            }

            // Se llama al proveedor específico y se procesa su resultado.
            return provider.GeocodeAddressAsync(address)
                .ToList()
                .Select(list =>
                {
                    var first = list.FirstOrDefault();
                    if (first == null)
                    {
                        _logger.LogError($"[Aggregator] El proveedor {providerName} no devolvió resultados.");
                        throw new InvalidOperationException($"El proveedor {providerName} no devolvió resultados.");
                    }
                    return first;
                });
        }
    }
}