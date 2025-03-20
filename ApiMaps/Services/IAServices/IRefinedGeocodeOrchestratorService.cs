using ApiMaps.Models.DTOs.GeocodeResponseDtos;

namespace ApiMaps.Services.IAServices
{
    /// <summary>
    /// Servicio orquestador que combina:
    /// 1) Refinamiento de direcciones vía IA (opcional).
    /// 2) Geocodificación usando el <see cref="IGeocodeService"/>.
    /// 3) Búsqueda de lugares cercanos con <see cref="IPlacesService"/>.
    /// Se salta pasos si falta configuración (API keys, endpoints) y registra logs del proceso.
    /// </summary>
    public interface IRefinedGeocodeOrchestratorService
    {
        /// <summary>
        /// Procesa una dirección aplicando IA (si está configurada),
        /// luego la geocodifica y finalmente (opcional) busca lugares cercanos en Google Places.
        /// Devuelve un <see cref="IObservable{T}"/> con un objeto que encapsula todo.
        /// </summary>
        /// <param name="address">Dirección original a refinar y geocodificar.</param>
        /// <param name="placesRadius">Radio (metros) para buscar lugares cercanos. 0 o < 0 para saltarlo.</param>
        /// <returns>
        /// Un observable que emite un <see cref="RefinedGeocodeResponseDto"/> con la información resultante.
        /// </returns>
        IObservable<RefinedGeocodeResponseDto> ProcessRefinedAddressAsync(string address, int placesRadius);
    }
    
}
