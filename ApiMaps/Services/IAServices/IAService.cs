using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using ApiMaps.Services.LoggingServices;
using ApiMaps.Services.ParameterServices;

namespace ApiMaps.Services.IAServices
{
    /// <summary>
    /// Implementación de <see cref="IIaService"/> que, de forma genérica,
    /// permite refinar direcciones usando un servicio de IA externo 
    /// (ej.: OpenAI, Copilot u otro).
    /// </summary>
    public class IaService : IIaService
    {
        private readonly IParameterService _parameterService;
        private readonly ILoggerService<IaService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Crea una nueva instancia de <see cref="IaService"/>.
        /// </summary>
        /// <param name="parameterService">
        /// Servicio para acceder a los parámetros de configuración en la base de datos 
        /// (por ejemplo, clave y endpoint de IA).
        /// </param>
        /// <param name="logger">
        /// Servicio de logging para registrar eventos e información de depuración.
        /// </param>
        /// <param name="httpClientFactory">
        /// Fábrica de clientes HTTP para enviar solicitudes al servicio de IA externo.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="parameterService"/>, <paramref name="logger"/> 
        /// o <paramref name="httpClientFactory"/> son nulos.
        /// </exception>
        public IaService(
            IParameterService parameterService,
            ILoggerService<IaService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _parameterService = parameterService
                ?? throw new ArgumentNullException(nameof(parameterService));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory
                ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <inheritdoc />
        public IObservable<string> CorrectAddressSpellingAsync(string address)
        {
            return Observable.Return(address);
        }

        /// <inheritdoc />
        public IObservable<string> DisambiguateAddressAsync(string address)
        {
            return Observable.Return(address);
        }

        /// <inheritdoc />
        public IObservable<string> PrioritizeContextualAsync(string address, IEnumerable<string> contextKeywords)
        {
            return Observable.Return(address);
        }

        /// <inheritdoc />
        /// <summary>
        /// Invoca a la API de IA de forma genérica 
        /// (por ejemplo, ChatGPT u otro proveedor), enviando un prompt
        /// para refinar la dirección. Usa parámetros como "IAEndpoint" y "ApiKeyIA".
        /// </summary>
        public IObservable<string> RefineAddressWithIaAsync(string address)
        {
            return Observable.FromAsync(async () =>
            {
                // 1) Obtener parámetros de la DB
                var iaKeyParam = await _parameterService.GetByNameAsync("ApiKeyIA").FirstOrDefaultAsync();
                if (iaKeyParam == null)
                {
                    throw new InvalidOperationException("No se encontró configuración para 'ApiKeyIA'.");
                }

                var iaEndpointParam = await _parameterService.GetByNameAsync("IAEndpoint").FirstOrDefaultAsync();
                if (iaEndpointParam == null)
                {
                    throw new InvalidOperationException("No se encontró configuración para 'IAEndpoint'.");
                }

                // Puedes también usar un "IAModel" (opcional)
                var iaModelParam = await _parameterService.GetByNameAsync("IAModel").FirstOrDefaultAsync();
                var defaultModel = "default-model";
                var iaModel = iaModelParam?.ParameterValue ?? defaultModel;

                var iaKey = iaKeyParam.ParameterValue;
                var endpointUrl = iaEndpointParam.ParameterValue;

                _logger.LogInfo($"[IaService] Refinar dirección usando IA (endpoint: {endpointUrl}). Dirección original: {address}");

                var requestBody = new
                {
                    model = iaModel,
                    prompt = $"Refina esta dirección: {address}",
                    max_tokens = 100
                };

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {iaKey}");

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpointUrl, httpContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInfo($"[IaService] Respuesta de IA: {responseContent}");

                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                var choices = root.GetProperty("choices");
                var firstChoice = choices[0];
                var refinedAddress = firstChoice.GetProperty("text").GetString();

                return refinedAddress ?? address;
            });
        }
    }
}
