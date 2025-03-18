namespace ApiMaps.Services.GeocodingServices;

public interface IGeocodingProviderFactory
{
    /// <summary>
    /// Crea dinámicamente una instancia de un proveedor de geocodificación para el proveedor indicado.
    /// </summary>
    IObservable<IGeocodingProvider> CreateProviderAsync(string providerName);
}
