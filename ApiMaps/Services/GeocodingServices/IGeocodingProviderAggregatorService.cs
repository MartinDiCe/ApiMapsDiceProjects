using ApiMaps.Models.DTOs.GeocodeResponseDtos;

namespace ApiMaps.Services.GeocodingServices
{
    /// <summary>
    /// Define las operaciones para un agregador de proveedores de geocodificación.
    /// Este agregador puede:
    /// - Obtener un único resultado basado en la mayor prioridad (valor numérico menor).
    /// - Invocar a un proveedor específico por nombre.
    /// - Invocar a todos los proveedores y combinar sus resultados.
    /// - Agrupar resultados por proveedor, filtrando según prioridades.
    /// </summary>
    public interface IGeocodingProviderAggregatorService
    {
        /// <summary>
        /// Procesa una dirección utilizando los proveedores con la mayor prioridad (valor numérico menor).
        /// </summary>
        /// <param name="address">La dirección a geocodificar.</param>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite un <see cref="GeocodeResponseDto"/> con el primer resultado exitoso
        /// entre los proveedores de mayor prioridad, o lanza una excepción si no se obtienen resultados.
        /// </returns>
        IObservable<GeocodeResponseDto> GeocodeAddressAsync(string address);

        /// <summary>
        /// Llama a un proveedor específico (identificado por <paramref name="providerName"/>) y devuelve su primer resultado exitoso.
        /// </summary>
        /// <param name="address">La dirección a geocodificar.</param>
        /// <param name="providerName">Nombre del proveedor específico a utilizar.</param>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite el primer <see cref="GeocodeResponseDto"/> resultante del proveedor específico.
        /// Lanza una excepción si el proveedor no existe o no retorna ningún resultado.
        /// </returns>
        IObservable<GeocodeResponseDto> GeocodeAddressByProviderAsync(string address, string providerName);

        /// <summary>
        /// Llama a todos los proveedores disponibles y combina todos sus resultados.
        /// </summary>
        /// <param name="address">La dirección a geocodificar.</param>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite una lista con los <see cref="GeocodeResponseDto"/> resultantes
        /// de todos los proveedores.
        /// </returns>
        IObservable<IList<GeocodeResponseDto>> GeocodeAddressAllAsync(string address);

        /// <summary>
        /// Llama a los proveedores que cumplen con las prioridades indicadas y agrupa sus resultados
        /// en un diccionario, donde cada clave es el nombre del proveedor y el valor es una lista de resultados.
        /// </summary>
        /// <param name="address">La dirección a geocodificar.</param>
        /// <param name="priorities">
        /// Colección de prioridades a considerar. Por ejemplo, [1] o [1, 2].
        /// Un proveedor será llamado solo si su prioridad está en esta lista.
        /// </param>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite un diccionario. Cada clave es el nombre del proveedor y 
        /// el valor es una lista de <see cref="GeocodeResponseDto"/>.
        /// </returns>
        IObservable<IDictionary<string, IList<GeocodeResponseDto>>> GeocodeAddressGroupedAsync(
            string address,
            IEnumerable<int> priorities
        );
    }
}