using System.Reactive.Linq;
using ApiMaps.Services.LoggingServices;
using ApiMaps.Services.ProviderConfigServices;

namespace ApiMaps.Services.GeocodingServices
{
    /// <summary>
    /// Fábrica que crea dinámicamente instancias de <see cref="IGeocodingProvider"/> usando la configuración obtenida desde la base de datos.
    /// </summary>
    /// <remarks>
    /// La fábrica utiliza el servicio <see cref="IProviderConfigurationService"/> para obtener las opciones de configuración del proveedor (como el endpoint, API key, y prioridad),
    /// luego utiliza un <see cref="IHttpClientFactory"/> para crear un cliente HTTP y finalmente instancia un <see cref="GeocodingProviderService"/>.
    /// Se registran logs en cada paso para facilitar el seguimiento del proceso de creación.
    /// </remarks>
    public class GeocodingProviderFactory : IGeocodingProviderFactory
    {
        private readonly IProviderConfigurationService _providerConfigurationService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerService<GeocodingProviderService> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="GeocodingProviderFactory"/>.
        /// </summary>
        /// <param name="providerConfigurationService">
        /// Servicio que obtiene la configuración de proveedores desde la base de datos.
        /// </param>
        /// <param name="httpClientFactory">
        /// Fábrica para crear instancias de <see cref="HttpClient"/>.
        /// </param>
        /// <param name="logger">
        /// Servicio de logging para el proveedor de geocodificación.
        /// </param>
        public GeocodingProviderFactory(
            IProviderConfigurationService providerConfigurationService,
            IHttpClientFactory httpClientFactory,
            ILoggerService<GeocodingProviderService> logger)
        {
            _providerConfigurationService = providerConfigurationService ?? throw new ArgumentNullException(nameof(providerConfigurationService));
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
            _logger.LogInfo($"[Factory] Iniciando creación del proveedor de geocodificación para: {providerName}");
            return _providerConfigurationService.GetProviderOptionsAsync(providerName)
                .Select(options =>
                {
                    _logger.LogInfo($"[Factory] Configuración obtenida para {options.ProviderName}: Endpoint = {options.EndpointTemplate}, Priority = {options.Priority}");
                    var httpClient = _httpClientFactory.CreateClient();
                    
                    // Aquí se crea la instancia del proveedor individual usando la configuración
                    var provider = new GeocodingProviderService(
                        httpClient,
                        _logger,
                        options.ProviderName,
                        options.EndpointTemplate,
                        options.ApiKey,
                        options.Priority
                    );
                    
                    _logger.LogInfo($"[Factory] Proveedor {options.ProviderName} creado correctamente.");
                    return (IGeocodingProvider)provider;
                });
        }
    }
}