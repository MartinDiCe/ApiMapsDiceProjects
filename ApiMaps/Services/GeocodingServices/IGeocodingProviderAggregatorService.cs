using ApiMaps.Models.DTOs.GeocodeResponseDtos;

namespace ApiMaps.Services.GeocodingServices
{
    /// <summary>
    /// Define operaciones adicionales para un agregador de proveedores de geocodificación.
    /// Este agregador extiende la funcionalidad básica de <see cref="IGeocodingProvider"/>
    /// para permitir obtener todos los resultados de un proveedor específico, de todos los proveedores,
    /// o agrupar los resultados por proveedor según las prioridades indicadas.
    /// </summary>
    public interface IGeocodingProviderAggregatorService : IGeocodingProvider
    {
        /// <summary>
        /// Llama al proveedor específico indicado y devuelve todos sus resultados.
        /// </summary>
        /// <param name="address">La dirección a geocodificar.</param>
        /// <param name="providerName">El nombre del proveedor que se desea utilizar.</param>
        /// <returns>
        /// Un observable que emite una lista con las respuestas de geocodificación del proveedor especificado.
        /// </returns>
        IObservable<GeocodeResponseDto> GeocodeAddressByProviderAsync(string address, string providerName);

        /// <summary>
        /// Llama a todos los proveedores y combina sus resultados en una lista.
        /// </summary>
        /// <param name="address">La dirección a geocodificar.</param>
        /// <returns>
        /// Un observable que emite una lista con las respuestas de geocodificación de todos los proveedores.
        /// </returns>
        IObservable<IList<GeocodeResponseDto>> GeocodeAddressAllAsync(string address);

        /// <summary>
        /// Llama a los proveedores que cumplen con las prioridades indicadas y agrupa sus resultados en un diccionario.
        /// Cada clave es el nombre del proveedor y el valor es la lista de respuestas obtenidas.
        /// </summary>
        /// <param name="address">La dirección a geocodificar.</param>
        /// <param name="priorities">
        /// Una colección de enteros que representa las prioridades a considerar (por ejemplo, [1] o [1,2]).
        /// </param>
        /// <returns>
        /// Un observable que emite un diccionario donde cada clave es el nombre del proveedor y el valor es una lista de resultados.
        /// </returns>
        IObservable<IDictionary<string, IList<GeocodeResponseDto>>> GeocodeAddressGroupedAsync(string address, IEnumerable<int> priorities);
    }
}
