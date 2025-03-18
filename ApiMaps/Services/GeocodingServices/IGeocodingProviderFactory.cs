namespace ApiMaps.Services.GeocodingServices
{
    /// <summary>
    /// Define métodos para crear instancias de proveedores de geocodificación (<see cref="IGeocodingProvider"/>)
    /// de forma dinámica a partir de la configuración almacenada en la base de datos.
    /// </summary>
    public interface IGeocodingProviderFactory
    {
        /// <summary>
        /// Crea dinámicamente una instancia de un proveedor de geocodificación para el proveedor indicado.
        /// </summary>
        /// <param name="providerName">
        /// Nombre del proveedor (por ejemplo, "GoogleMaps") cuya configuración se desea obtener de la base de datos.
        /// </param>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite la instancia de <see cref="IGeocodingProvider"/> configurada.
        /// En caso de no encontrarse configuración para el <paramref name="providerName"/>,
        /// puede lanzar una excepción (<see cref="InvalidOperationException"/> u otra).
        /// </returns>
        IObservable<IGeocodingProvider> CreateProviderAsync(string providerName);

        /// <summary>
        /// Crea instancias de <see cref="IGeocodingProvider"/> para todos los proveedores registrados
        /// en la base de datos (tabla de configuraciones).
        /// </summary>
        /// <returns>
        /// Un <see cref="IObservable{T}"/> que emite una colección de todos los proveedores configurados.
        /// Cada proveedor se construye dinámicamente con la información de la base de datos
        /// (endpoint, API key, prioridad, etc.).
        /// </returns>
        IObservable<IEnumerable<IGeocodingProvider>> CreateAllProvidersAsync();
    }
}
