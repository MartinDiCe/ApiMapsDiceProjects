using System.Reactive.Linq;
using ApiMaps.Services.ApiMapConfigServices;
using ApiMaps.Services.LoggingServices;

namespace ApiMaps.Services.GeocodingServices
{
    /// <summary>
    /// Fábrica que crea dinámicamente instancias de <see cref="IGeocodingProvider"/> 
    /// a partir de la configuración almacenada en la base de datos.
    /// </summary>
    /// <remarks>
    /// Esta versión expone dos métodos:
    /// 1. <see cref="CreateProviderAsync(string)"/> para crear un proveedor específico
    ///    (por ejemplo, "GoogleMaps").
    /// 2. <see cref="CreateAllProvidersAsync()"/> para construir instancias de todos 
    ///    los proveedores configurados en la tabla <c>ApiMapConfigs</c>.
    /// </remarks>
    public class GeocodingProviderFactory : IGeocodingProviderFactory
    {
        private readonly IApiMapConfigService _apiMapConfigService;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILoggerService<GeocodingProviderService> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="GeocodingProviderFactory"/>.
        /// </summary>
        /// <param name="apiMapConfigService">
        /// Servicio para acceder a la lista de configuraciones de proveedores en DB.
        /// </param>
        /// <param name="httpClientFactory">
        /// Fábrica para crear instancias de <see cref="HttpClient"/>.
        /// </param>
        /// <param name="logger">
        /// Servicio de logging para los mensajes de geocodificación.
        /// </param>
        public GeocodingProviderFactory(
            IApiMapConfigService apiMapConfigService,
            IHttpClientFactory httpClientFactory,
            ILoggerService<GeocodingProviderService> logger)
        {
            _apiMapConfigService = apiMapConfigService ?? throw new ArgumentNullException(nameof(apiMapConfigService));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea dinámicamente una instancia de <see cref="IGeocodingProvider"/> para el proveedor especificado.
        /// </summary>
        /// <param name="providerName">Nombre del proveedor cuya configuración se desea utilizar.</param>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite la instancia de <see cref="IGeocodingProvider"/> configurada.
        /// </returns>
        public IObservable<IGeocodingProvider> CreateProviderAsync(string providerName)
        {
            _logger.LogInfo($"[Factory] Iniciando creación de proveedor para: {providerName}");

            return _apiMapConfigService.GetAllAsync()
                .Select(configs =>
                {
                    var config = configs.FirstOrDefault(c =>
                        c.Proveedor.Equals(providerName, StringComparison.OrdinalIgnoreCase));

                    if (config == null)
                    {
                        throw new InvalidOperationException(
                            $"No se encontró configuración para el proveedor '{providerName}'.");
                    }

                    _logger.LogInfo(
                        $"[Factory] Configuración obtenida para {config.Proveedor}: " +
                        $"Endpoint = {config.EndPoint}, Priority = {config.Prioridad}");

                    var httpClient = _httpClientFactory.CreateClient();
                    var provider = new GeocodingProviderService(
                        httpClient,
                        _logger,
                        config.Proveedor,
                        config.EndPoint,
                        config.ApiKey,
                        config.Prioridad
                    );

                    _logger.LogInfo($"[Factory] Proveedor {providerName} creado correctamente.");

                    return (IGeocodingProvider)provider;
                });
        }

        /// <summary>
        /// Crea instancias de <see cref="IGeocodingProvider"/> para todos los proveedores
        /// registrados en la base de datos.
        /// </summary>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite un listado de todos los proveedores configurados.
        /// </returns>
        public IObservable<IEnumerable<IGeocodingProvider>> CreateAllProvidersAsync()
        {
            _logger.LogInfo("[Factory] Iniciando creación de TODOS los proveedores de geocodificación...");

            return _apiMapConfigService.GetAllAsync()
                .Select(configs =>
                {
                    var providers = new List<IGeocodingProvider>();

                    foreach (var cfg in configs)
                    {
                        var httpClient = _httpClientFactory.CreateClient();
                        var provider = new GeocodingProviderService(
                            httpClient,
                            _logger,
                            cfg.Proveedor,
                            cfg.EndPoint,
                            cfg.ApiKey,
                            cfg.Prioridad
                        );
                        providers.Add(provider);
                    }

                    _logger.LogInfo($"[Factory] Se crearon {providers.Count} proveedores en total.");
                    return (IEnumerable<IGeocodingProvider>)providers;
                });
        }
    }
}