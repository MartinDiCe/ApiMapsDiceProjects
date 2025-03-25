using System.Reactive.Linq;
using ApiMaps.Models.DTOs.IADtos;
using ApiMaps.Services.LoggingServices;
using ApiMaps.Services.ParameterServices;
using System.Text.Json;

namespace ApiMaps.Services.PlacesServices
{
    /// <summary>
    /// Define la lógica para consultar la API de Google Places,
    /// tanto para la búsqueda de lugares cercanos (Nearby Search) 
    /// como para obtener detalles de un lugar específico (Place Details).
    /// </summary>
    public class PlacesService : IPlacesService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IParameterService _parameterService;
        private readonly ILoggerService<PlacesService> _logger;

        /// <summary>
        /// Constructor de <see cref="PlacesService"/>, 
        /// inyecta dependencias para el acceso a la configuración, logging y cliente HTTP.
        /// </summary>
        /// <param name="httpClientFactory">
        /// Fábrica de clientes HTTP para realizar las solicitudes a la API de Google.
        /// </param>
        /// <param name="parameterService">
        /// Servicio para obtener parámetros de configuración, 
        /// incluyendo la API Key de Google Places.
        /// </param>
        /// <param name="logger">
        /// Servicio de logging para registrar la actividad y errores.
        /// </param>
        public PlacesService(
            IHttpClientFactory httpClientFactory,
            IParameterService parameterService,
            ILoggerService<PlacesService> logger)
        {
            _httpClientFactory = httpClientFactory 
                ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _parameterService = parameterService 
                ?? throw new ArgumentNullException(nameof(parameterService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public IObservable<PlacesResultDto> SearchNearbyAsync(double lat, double lng, int radiusMeters)
        {
            return Observable.FromAsync(async () =>
            {
                var placesKey = await _parameterService
                    .GetByNameAsync("PlacesApiKey")
                    .FirstAsync();

                if (placesKey == null)
                {
                    throw new InvalidOperationException(
                        "No se encontró configuración para 'PlacesApiKey'.");
                }

                var placesEndpoint = await _parameterService
                    .GetByNameAsync("PlacesEndpoint")
                    .FirstAsync();

                if (placesEndpoint == null)
                {
                    throw new InvalidOperationException(
                        "No se encontró configuración para 'PlacesEndpoint'.");
                }

                string baseUrl = placesEndpoint.ParameterValue;
                
                string requestUrl = $"{baseUrl}?location={lat},{lng}&radius={radiusMeters}&key={placesKey.ParameterValue}";

                _logger.LogInfo($"[PlacesService] Request a: {requestUrl}");

                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInfo($"[PlacesService] Respuesta: {content}");

                var result = JsonSerializer.Deserialize<PlacesResultDto>(content);
                if (result == null)
                {
                    throw new InvalidOperationException("No se pudo deserializar PlacesResultDto.");
                }

                return result;
            });
        }

        /// <inheritdoc />
        public IObservable<PlacesDetailDto> GetPlaceDetailsAsync(string placeId)
        {
            return Observable.FromAsync(async () =>
            {
                var placesKey = await _parameterService
                    .GetByNameAsync("PlacesApiKey")
                    .FirstAsync();

                if (placesKey == null)
                {
                    throw new InvalidOperationException(
                        "No se encontró configuración para 'PlacesApiKey'.");
                }

                var placesEndpoint = await _parameterService
                    .GetByNameAsync("placesDetailsEndpoint")
                    .FirstAsync();

                if (placesEndpoint == null)
                {
                    throw new InvalidOperationException(
                        "No se encontró configuración para 'PlacesDetailsEndpoint'.");
                }

                string baseUrl = placesEndpoint.ParameterValue;
                string requestUrl = $"{baseUrl}?place_id={placeId}&key={placesKey.ParameterValue}";

                _logger.LogInfo($"[PlacesService] Detalle de PlaceID: {placeId}. Request a: {requestUrl}");

                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInfo($"[PlacesService] Respuesta detalle: {content}");

                var detailResult = JsonSerializer.Deserialize<PlacesDetailDto>(content);
                if (detailResult == null)
                {
                    throw new InvalidOperationException("No se pudo deserializar PlacesDetailDto.");
                }

                return detailResult;
            });
        }
    }
}
