using ApiMaps.Models.DTOs.GeocodeResponseDtos;
using ApiMaps.Services.LoggingServices;

namespace ApiMaps.Services.GeocodingServices
{
    /// <summary>
    /// Servicio central de geocodificación que orquesta el flujo completo.
    /// </summary>
    public class GeocodeService(
        IGeocodingProviderAggregatorService providerAggregator,
        ILoggerService<GeocodeService> logger)
        : IGeocodeService
    {
        private readonly IGeocodingProviderAggregatorService _providerAggregator = providerAggregator ?? throw new ArgumentNullException(nameof(providerAggregator));
        private readonly ILoggerService<GeocodeService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public IObservable<GeocodeResponseDto> ProcessAddressAsync(string address)
        {
            _logger.LogInfo($"[GeocodeService] Procesando dirección: {address}");
            return _providerAggregator.GeocodeAddressAsync(address);
        }

        public IObservable<GeocodeResponseDto> ProcessAddressByProviderAsync(string address, string providerName)
        {
            _logger.LogInfo($"[GeocodeService] Procesando dirección: {address} usando proveedor: {providerName}");
            return _providerAggregator.GeocodeAddressByProviderAsync(address, providerName);
        }

        public IObservable<IList<GeocodeResponseDto>> ProcessAddressAllAsync(string address)
        {
            _logger.LogInfo($"[GeocodeService] Procesando dirección para todos los proveedores: {address}");
            return _providerAggregator.GeocodeAddressAllAsync(address);
        }
        
        public IObservable<IDictionary<string, IList<GeocodeResponseDto>>> ProcessAddressGroupedAsync(string address, IEnumerable<int> priorities)
        {
            _logger.LogInfo($"[GeocodeService] Procesando dirección agrupada: {address}");
            return _providerAggregator.GeocodeAddressGroupedAsync(address, priorities);
        }
        
    }
}