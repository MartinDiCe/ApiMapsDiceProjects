using ApiMaps.Models.DTOs.IADtos;

namespace ApiMaps.Services.PlacesServices
{
    /// <summary>
    /// Interfaz que define las operaciones para interactuar con la API de Google Places
    /// de manera 100% reactiva.
    /// </summary>
    public interface IPlacesService
    {
        /// <summary>
        /// Realiza una búsqueda de lugares cercanos (Nearby Search) 
        /// a partir de una ubicación (lat, lng) y un radio en metros.
        /// </summary>
        /// <param name="lat">Latitud de la ubicación.</param>
        /// <param name="lng">Longitud de la ubicación.</param>
        /// <param name="radiusMeters">Radio de búsqueda en metros.</param>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite un <see cref="PlacesResultDto"/>
        /// con los resultados de la búsqueda (lista de lugares).
        /// </returns>
        IObservable<PlacesResultDto> SearchNearbyAsync(double lat, double lng, int radiusMeters);

        /// <summary>
        /// Obtiene los detalles de un lugar (Place Details) a partir de su <paramref name="placeId"/>.
        /// </summary>
        /// <param name="placeId">Identificador único del lugar devuelto por la API de Google Places.</param>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite un <see cref="PlacesDetailDto"/>
        /// con la información detallada del lugar (dirección, nombre, teléfono, etc.).
        /// </returns>
        IObservable<PlacesDetailDto> GetPlaceDetailsAsync(string placeId);
    }
}
