using ApiMaps.Models.DTOs.ApiMapConfigDtos;

namespace ApiMaps.Services.ProviderConfigServices

{
    /// <summary>
    /// Define las operaciones para obtener la configuración de un proveedor de mapas de forma reactiva.
    /// </summary>
    public interface IProviderConfigurationService
    {
        /// <summary>
        /// Obtiene las opciones de configuración para un proveedor de geocodificación.
        /// </summary>
        /// <param name="providerName">Nombre del proveedor.</param>
        /// <returns>
        /// Un observable que emite las opciones de configuración.
        /// </returns>
        IObservable<GeocodingProviderOptions> GetProviderOptionsAsync(string providerName);
        
    }
}