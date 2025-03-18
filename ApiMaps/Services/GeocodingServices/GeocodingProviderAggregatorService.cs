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
    /// En este enfoque, el agregador llama de forma dinámica a la fábrica para construir la lista de
    /// proveedores al momento de cada solicitud, en lugar de recibirlos en el constructor.
    /// </remarks>
    public class GeocodingProviderAggregatorService : IGeocodingProviderAggregatorService
    {
        private readonly IGeocodingProviderFactory _factory;
        private readonly ILoggerService<GeocodingProviderAggregatorService> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="GeocodingProviderAggregatorService"/>.
        /// </summary>
        /// <param name="factory">Fábrica que construye dinámicamente los proveedores de geocodificación.</param>
        /// <param name="logger">Servicio de logging para el agregador.</param>
        public GeocodingProviderAggregatorService(
            IGeocodingProviderFactory factory,
            ILoggerService<GeocodingProviderAggregatorService> logger)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Nombre del proveedor. Se identifica como "Aggregator".
        /// </summary>
        public string ProviderName => "Aggregator";

        // Propiedad Priority opcional, o se puede eliminar si no es necesaria.
        // public int Priority => ???

        /// <inheritdoc/>
        public IObservable<GeocodeResponseDto> GeocodeAddressAsync(string address)
        {
            _logger.LogInfo($"[Aggregator] Procesando dirección: {address}");

            // 1) Obtenemos todos los proveedores de forma dinámica.
            return _factory
                .CreateAllProvidersAsync() 
                .SelectMany(providers =>
                {
                    // 2) Revisar que no esté vacío.
                    if (!providers.Any())
                        throw new InvalidOperationException("No hay proveedores configurados.");

                    // 3) Seleccionar la PRIORIDAD más alta (valor numérico mínimo).
                    int minPriority = providers.Min(p => p.Priority);
                    var selectedProviders = providers
                        .Where(p => p.Priority == minPriority)
                        .ToList();

                    _logger.LogInfo($"[Aggregator] Se seleccionaron {selectedProviders.Count} proveedor(es) con prioridad {minPriority}");

                    // 4) Para cada proveedor seleccionado, se llama a su método de geocodificación y se capturan errores.
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

                    // 5) Combinamos (merge) los resultados; si no se produce ninguno, lanzamos un error controlado.
                    return observables
                        .Merge()
                        .DefaultIfEmpty(null)
                        .Select(result =>
                        {
                            if (result == null)
                                throw new InvalidOperationException("No se encontraron resultados para la dirección proporcionada.");
                            return result;
                        });
                });
        }

        /// <inheritdoc/>
        public IObservable<IList<GeocodeResponseDto>> GeocodeAddressAllAsync(string address)
        {
            _logger.LogInfo($"[Aggregator] Procesando dirección para todos los proveedores: {address}");

            return _factory
                .CreateAllProvidersAsync()
                .SelectMany(providers =>
                {
                    if (!providers.Any())
                        throw new InvalidOperationException("No hay proveedores configurados.");

                    var observables = providers.Select(provider =>
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
                });
        }

        /// <inheritdoc/>
        public IObservable<IDictionary<string, IList<GeocodeResponseDto>>> GeocodeAddressGroupedAsync(
            string address,
            IEnumerable<int> priorities)
        {
            var priorityList = priorities.ToList();
            _logger.LogInfo($"[Aggregator] Procesando dirección para prioridades: {string.Join(",", priorityList)}");

            return _factory
                .CreateAllProvidersAsync()
                .SelectMany(providers =>
                {
                    // Filtrar solo los proveedores que tengan una prioridad incluida en 'priorityList'.
                    var selectedProviders = providers
                        .Where(p => priorityList.Contains(p.Priority))
                        .ToList();

                    _logger.LogInfo($"[Aggregator] Se seleccionaron {selectedProviders.Count} proveedor(es).");

                    // Para cada proveedor, llamamos GeocodeAddressAsync y agrupamos los resultados.
                    var observables = selectedProviders.Select(provider =>
                        provider.GeocodeAddressAsync(address)
                            .Catch<GeocodeResponseDto, Exception>(ex =>
                            {
                                _logger.LogError($"[Aggregator] Error en {provider.ProviderName}: {ex.Message}");
                                return Observable.Empty<GeocodeResponseDto>();
                            })
                            .Select(result => new { provider.ProviderName, Result = result })
                    );

                    return observables
                        .Merge()
                        .ToList()
                        .Select(results =>
                        {
                            var dict = results
                                .GroupBy(x => x.ProviderName)
                                .ToDictionary(
                                    g => g.Key,
                                    g => (IList<GeocodeResponseDto>)g.Select(x => x.Result).ToList()
                                );
                            return (IDictionary<string, IList<GeocodeResponseDto>>)dict;
                        });
                });
        }

        /// <inheritdoc/>
        public IObservable<GeocodeResponseDto> GeocodeAddressByProviderAsync(string address, string providerName)
        {
            _logger.LogInfo($"[Aggregator] Buscando proveedor: {providerName} para {address}");

            return _factory
                .CreateAllProvidersAsync()
                .SelectMany(providers =>
                {
                    var provider = providers
                        .FirstOrDefault(p => p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

                    if (provider == null)
                    {
                        _logger.LogError($"[Aggregator] Proveedor {providerName} no encontrado.");
                        return Observable.Throw<GeocodeResponseDto>(
                            new ArgumentException($"Proveedor {providerName} no encontrado.")
                        );
                    }

                    // Llamar al proveedor específico y obtener el primer resultado.
                    return provider.GeocodeAddressAsync(address)
                        .ToList()
                        .Select(list =>
                        {
                            var first = list.FirstOrDefault();
                            if (first == null)
                            {
                                _logger.LogError($"[Aggregator] {providerName} no devolvió resultados.");
                                throw new InvalidOperationException($"{providerName} no devolvió resultados.");
                            }
                            return first;
                        });
                });
        }
    }
}
