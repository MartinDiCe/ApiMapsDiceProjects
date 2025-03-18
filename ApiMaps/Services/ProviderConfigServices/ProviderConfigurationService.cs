using System.Reactive.Linq;
using ApiMaps.Models.DTOs.ApiMapConfigDtos;
using ApiMaps.Services.ApiMapConfigServices;
using ApiMaps.Services.LoggingServices;

namespace ApiMaps.Services.ProviderConfigServices
{
    /// <summary>
    /// Implementa la obtención reactiva de la configuración de proveedores desde la base de datos.
    /// </summary>
    /// <remarks>
    /// Este servicio utiliza <see cref="IApiMapConfigService"/> para obtener todas las configuraciones almacenadas,
    /// filtra por el proveedor solicitado y transforma el resultado en un objeto <see cref="GeocodingProviderOptions"/>.
    /// Se incluyen logs para rastrear el proceso de obtención y filtrado.
    /// </remarks>
    public class ProviderConfigurationService : IProviderConfigurationService
    {
        private readonly IApiMapConfigService _apiMapConfigService;
        private readonly ILoggerService<ProviderConfigurationService> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ProviderConfigurationService"/>.
        /// </summary>
        /// <param name="apiMapConfigService">
        /// Servicio para acceder a la configuración de proveedores almacenada en la base de datos.
        /// </param>
        /// <param name="logger">Servicio de logging para registrar el proceso de obtención de la configuración.</param>
        public ProviderConfigurationService(IApiMapConfigService apiMapConfigService, ILoggerService<ProviderConfigurationService> logger)
        {
            _apiMapConfigService = apiMapConfigService ?? throw new ArgumentNullException(nameof(apiMapConfigService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public IObservable<GeocodingProviderOptions> GetProviderOptionsAsync(string providerName)
        {
            _logger.LogInfo($"[ProviderConfigurationService] Solicitando configuración para el proveedor: {providerName}");
            return _apiMapConfigService.GetAllAsync()
                .Select(configs =>
                {
                    _logger.LogInfo($"[ProviderConfigurationService] Se obtuvieron {configs.Count()} configuraciones de proveedores.");
                    var config = configs.FirstOrDefault(c =>
                        c.Proveedor.Equals(providerName, StringComparison.OrdinalIgnoreCase));
                    
                    if (config == null)
                    {
                        _logger.LogError($"[ProviderConfigurationService] No se encontró configuración para el proveedor {providerName}");
                        throw new Exception($"No se encontró configuración para el proveedor {providerName}");
                    }

                    _logger.LogInfo($"[ProviderConfigurationService] Configuración encontrada para {providerName}: Endpoint = {config.EndPoint}, Priority = {config.Prioridad}");
                    return new GeocodingProviderOptions
                    {
                        ProviderName = config.Proveedor,
                        EndpointTemplate = config.EndPoint,
                        ApiKey = config.ApiKey,
                        Priority = config.Prioridad
                    };
                });
        }
    }
}