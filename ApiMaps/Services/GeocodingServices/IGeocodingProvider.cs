using ApiMaps.Models.DTOs.GeocodeResponseDtos;

namespace ApiMaps.Services.GeocodingServices
{
    /// <summary>
    /// Define las operaciones para enviar una dirección a un proveedor específico de geocodificación.
    /// </summary>
    public interface IGeocodingProvider
    {
        /// <summary>
        /// Procesa la dirección y obtiene la respuesta del proveedor.
        /// </summary>
        IObservable<GeocodeResponseDto> GeocodeAddressAsync(string address);

        /// <summary>
        /// Nombre del proveedor (por ejemplo, "GoogleMaps").
        /// </summary>
        string ProviderName { get; }
        
        /// <summary>
        /// Prioridades de los mapas
        /// </summary>
        int Priority { get; }
    }
}