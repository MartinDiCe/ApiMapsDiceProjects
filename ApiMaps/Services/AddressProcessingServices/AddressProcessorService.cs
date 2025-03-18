using System.Reactive.Linq;
using ApiMaps.Helpers;

namespace ApiMaps.Services.AddressProcessingServices;

/// <summary>
/// Implementa <see cref="IAddressProcessorService"/> para limpiar y normalizar direcciones.
/// </summary>
public class AddressProcessorService : IAddressProcessorService
{
    /// <inheritdoc/>
    public IObservable<string> ProcessAddressAsync(string address)
    {
        return Observable.Start(() =>
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("La dirección no puede estar vacía.", nameof(address));
            }
            
            return AddressHelper.CleanAddress(address);
        });
    }
}