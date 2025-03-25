namespace ApiMaps.Services.IAServices
{
    /// <summary>
    /// Interfaz para el servicio de IA (inteligencia artificial).
    /// Permite diferentes métodos de refinamiento y corrección de direcciones.
    /// </summary>
    public interface IIaService
    {
        /// <summary>
        /// Corrige ortografía de la dirección.
        /// </summary>
        IObservable<string> CorrectAddressSpellingAsync(string address);

        /// <summary>
        /// Desambigua la dirección (ej., corrige ciudad, país).
        /// </summary>
        IObservable<string> DisambiguateAddressAsync(string address);

        /// <summary>
        /// Prioriza resultados según un contexto dado.
        /// </summary>
        IObservable<string> PrioritizeContextualAsync(string address, IEnumerable<string> contextKeywords);

        /// <summary>
        /// Invoca a la API de IA (por ejemplo, ChatGPT) de forma genérica,
        /// enviándole un prompt para refinar la dirección.
        /// </summary>
        /// <remarks>
        /// El método busca parámetros en la base de datos, como "IAEndpoint" y "ApiKeyIA",
        /// para no depender de un solo proveedor. La implementación de parsing del JSON 
        /// se puede ajustar según la API concreta.
        /// </remarks>
        IObservable<string> RefineAddressWithIaAsync(string address);
    }
}