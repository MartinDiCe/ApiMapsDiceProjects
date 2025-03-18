using ApiMaps.Models.DTOs.GeocodeResponseDtos;

namespace ApiMaps.Services.GeocodingServices
{
    /// <summary>
    /// Define las operaciones del servicio central de geocodificación.
    /// </summary>
    public interface IGeocodeService
    {
        /// <summary>
        /// Procesa una dirección utilizando el primer resultado exitoso de todos los proveedores.
        /// </summary>
        IObservable<GeocodeResponseDto> ProcessAddressAsync(string address);

        /// <summary>
        /// Procesa una dirección utilizando el proveedor específico indicado.
        /// </summary>
        IObservable<GeocodeResponseDto> ProcessAddressByProviderAsync(string address, string providerName);

        /// <summary>
        /// Procesa una dirección y retorna los resultados de todos los proveedores.
        /// </summary>
        IObservable<IList<GeocodeResponseDto>> ProcessAddressAllAsync(string address);
        
        /// <summary>
        /// Procesa una dirección y agrupa los resultados de los proveedores que cumplan con las prioridades indicadas.
        /// </summary>
        /// <param name="address">La dirección a geocodificar.</param>
        /// <param name="priorities">Colección de prioridades a considerar.</param>
        /// <returns>
        /// Un observable que emite un diccionario donde cada clave es el nombre del proveedor y el valor es la lista de respuestas.
        /// </returns>
        IObservable<IDictionary<string, IList<GeocodeResponseDto>>> ProcessAddressGroupedAsync(string address, IEnumerable<int> priorities);
        
    }
}