namespace ApiMaps.Services.AddressProcessingServices
{
    /// <summary>
    /// Define las operaciones para procesar (limpiar y normalizar) una dirección.
    /// </summary>
    public interface IAddressProcessorService
    {
        /// <summary>
        /// Procesa y limpia la dirección, eliminando espacios extra y caracteres no permitidos.
        /// </summary>
        /// <param name="address">La dirección a procesar.</param>
        /// <returns>
        /// Un observable que emite la dirección limpia y normalizada.
        /// </returns>
        IObservable<string> ProcessAddressAsync(string address);
    }
}