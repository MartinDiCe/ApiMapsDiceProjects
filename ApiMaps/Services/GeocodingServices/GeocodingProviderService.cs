using System.Reactive.Linq;
using System.Text.Json;
using ApiMaps.Models.DTOs.GeocodeResponseDtos;
using ApiMaps.Services.LoggingServices;

namespace ApiMaps.Services.GeocodingServices
{
    /// <summary>
    /// Implementa un proveedor genérico de geocodificación basado en una plantilla de endpoint.
    /// </summary>
    /// <remarks>
    /// Esta clase utiliza una plantilla que contiene marcadores como <c>{address}</c> y <c>{apiKey}</c>,
    /// los cuales son reemplazados por la dirección y la clave de API respectivamente. Se realiza una llamada HTTP
    /// para obtener la respuesta de geocodificación, se registra el proceso y se maneja cualquier error.
    /// </remarks>
    public class GeocodingProviderService : IGeocodingProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILoggerService<GeocodingProviderService> _logger;
        private readonly string _endpointTemplate;
        private readonly string _apiKey;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="GeocodingProviderService"/>.
        /// </summary>
        /// <param name="httpClient">Cliente HTTP para realizar llamadas al API.</param>
        /// <param name="logger">Servicio de logging para registrar el proceso de geocodificación.</param>
        /// <param name="providerName">Nombre del proveedor (por ejemplo, "GoogleMaps").</param>
        /// <param name="endpointTemplate">
        /// Plantilla del endpoint, por ejemplo:
        /// "https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={apiKey}".
        /// </param>
        /// <param name="apiKey">Clave de API para autenticar la solicitud.</param>
        /// <param name="priority">Prioridad del proveedor; un valor numérico menor indica mayor prioridad.</param>
        public GeocodingProviderService(
            HttpClient httpClient,
            ILoggerService<GeocodingProviderService> logger,
            string providerName,
            string endpointTemplate,
            string apiKey,
            int priority)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ProviderName = providerName ?? throw new ArgumentNullException(nameof(providerName));
            _endpointTemplate = endpointTemplate ?? throw new ArgumentNullException(nameof(endpointTemplate));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            Priority = priority;

            _logger.LogInfo($"[GeocodingProviderService] Instancia creada para {ProviderName} con endpoint '{_endpointTemplate}' y prioridad {Priority}.");
        }

        /// <inheritdoc/>
        public string ProviderName { get; }

        /// <summary>
        /// Prioridad del proveedor. Un valor numérico menor indica mayor prioridad.
        /// </summary>
        public int Priority { get; }

        /// <inheritdoc/>
        public IObservable<GeocodeResponseDto> GeocodeAddressAsync(string address)
        {
            return Observable.FromAsync(async () =>
            {
                
                var requestUri = _endpointTemplate
                    .Replace("{address}", Uri.EscapeDataString(address))
                    .Replace("{apiKey}", _apiKey);

                _logger.LogInfo($"[{ProviderName}] Llamando al endpoint: {requestUri}");

                var response = await _httpClient.GetAsync(requestUri);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"[{ProviderName}] Error en la llamada: {response.StatusCode}");
                    throw new HttpRequestException($"La llamada a {ProviderName} falló con código {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInfo($"[{ProviderName}] Respuesta recibida: {content}");

                var geocodeResponse = JsonSerializer.Deserialize<GeocodeResponseDto>(content);
                
                if (geocodeResponse == null)
                {
                    _logger.LogError($"[{ProviderName}] No se pudo deserializar la respuesta.");
                    throw new InvalidOperationException($"[{ProviderName}] No se pudo deserializar la respuesta.");
                }

                return geocodeResponse;
            });
        }
    }
}
