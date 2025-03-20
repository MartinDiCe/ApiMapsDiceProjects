using System.Reactive.Linq;
using ApiMaps.Services.GeocodingServices;
using ApiMaps.Services.PlacesServices;
using ApiMaps.Services.LoggingServices;
using ApiMaps.Models.DTOs.GeocodeResponseDtos;
using ApiMaps.Models.DTOs.IADtos;

namespace ApiMaps.Services.IAServices
{
    /// <summary>
    /// Implementación de <see cref="IRefinedGeocodeOrchestratorService"/> 
    /// que orquesta la IA, la geocodificación y la búsqueda de lugares.
    /// </summary>
    public class RefinedGeocodeOrchestratorService : IRefinedGeocodeOrchestratorService
    {
        private readonly IIaService _iaService;
        private readonly IGeocodeService _geocodeService;
        private readonly IPlacesService _placesService;
        private readonly ILoggerService<RefinedGeocodeOrchestratorService> _logger;

        /// <summary>
        /// Constructor que inyecta servicios de IA, geocodificación y Places,
        /// además de un logger para registrar la orquestación.
        /// </summary>
        public RefinedGeocodeOrchestratorService(
            IIaService iaService,
            IGeocodeService geocodeService,
            IPlacesService placesService,
            ILoggerService<RefinedGeocodeOrchestratorService> logger)
        {
            _iaService = iaService ?? throw new ArgumentNullException(nameof(iaService));
            _geocodeService = geocodeService ?? throw new ArgumentNullException(nameof(geocodeService));
            _placesService = placesService ?? throw new ArgumentNullException(nameof(placesService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public IObservable<RefinedGeocodeResponseDto> ProcessRefinedAddressAsync(string address, int placesRadius)
        {
            return Observable.Defer(() =>
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    return Observable.Throw<RefinedGeocodeResponseDto>(
                        new ArgumentException("La dirección es obligatoria.", nameof(address)));
                }

                _logger.LogInfo($"[RefinedOrchestrator] Iniciando proceso para '{address}', radio={placesRadius}");

                var resultDto = new RefinedGeocodeResponseDto
                {
                    OriginalAddress = address
                };

                // 1) Intentar IA
                return _iaService
                    .RefineAddressWithIaAsync(address)
                    .Catch<string, Exception>(ex =>
                    {
                        // Saltar IA si falla la config
                        resultDto.RefinedAddress = address; // Mantenemos original
                        resultDto.ProcessLogs.Add($"IAService: SKIPPED - {ex.Message}");
                        return Observable.Return(address); 
                    })
                    .Do(refined =>
                    {
                        if (refined != address)
                        {
                            resultDto.ProcessLogs.Add("IAService: Dirección refinada con éxito.");
                            resultDto.RefinedAddress = refined;
                        }
                        else
                        {
                            // Si no hubo error, pero la IA devolvió la original
                            resultDto.ProcessLogs.Add("IAService: Se usó la dirección original (sin cambios).");
                            resultDto.RefinedAddress = address;
                        }
                    })

                    // 2) Geocodificar la dirección (refinada o no)
                    .SelectMany(refined =>
                        _geocodeService.ProcessAddressAllAsync(refined)
                            .Catch<IList<GeocodeResponseDto>, Exception>(ex =>
                            {
                                // Si falla la geocodificación, lanzamos error
                                return Observable.Throw<IList<GeocodeResponseDto>>(ex);
                            })
                    )
                    .Do(geocodeResponses =>
                    {
                        resultDto.GeocodeResults = geocodeResponses;
                        resultDto.ProcessLogs.Add("GeocodeService: Ejecución completada (modo 'all').");

                        // Extraer la primera lat/lng "OK"
                        foreach (var geo in geocodeResponses)
                        {
                            if (geo.Status == "OK" && geo.Results.Any())
                            {
                                resultDto.Latitude = geo.Results.First().Geometry?.Location?.Latitude;
                                resultDto.Longitude = geo.Results.First().Geometry?.Location?.Longitude;
                                break;
                            }
                        }
                    })

                    // 3) Places (si hay lat/lng y radius > 0)
                    .SelectMany(_ =>
                    {
                        if (resultDto.Latitude.HasValue && resultDto.Longitude.HasValue && placesRadius > 0)
                        {
                            resultDto.UsedRadius = placesRadius;
                            return _placesService
                                .SearchNearbyAsync(resultDto.Latitude.Value, resultDto.Longitude.Value, placesRadius)
                                .Catch<PlacesResultDto, Exception>(ex =>
                                {
                                    // Si falla Places, lo saltamos
                                    resultDto.ProcessLogs.Add($"PlacesService: SKIPPED - {ex.Message}");
                                    return Observable.Return<PlacesResultDto>(null);
                                });
                        }
                        else
                        {
                            resultDto.ProcessLogs.Add("PlacesService: SKIPPED - No se contaba con lat/lng o placesRadius válido.");
                            return Observable.Return<PlacesResultDto>(null);
                        }
                    })
                    .Do(places =>
                    {
                        if (places != null)
                        {
                            resultDto.NearbyPlaces = places;
                            resultDto.ProcessLogs.Add("PlacesService: Búsqueda de lugares realizada con éxito.");
                        }
                    })

                    // 4) Devolver el objeto final
                    .Select(_ => resultDto);
            });
        }
    }
}
