using ApiMaps.Models.Entities;

namespace ApiMaps.Models.Repositories.ParameterRepositories
{
    /// <summary>
    /// Clase encargada de inicializar parámetros por defecto.
    /// </summary>
    public static class ParameterSeeder
    {
        /// <summary>
        /// Se ejecuta para verificar la existencia de parámetros por defecto y crearlos en caso de que no existan.
        /// </summary>
        /// <param name="parameterRepository">Repositorio de parámetros.</param>
        public static async Task SeedDefaultParametersAsync(IParameterRepository parameterRepository)
        {

            const string timeoutParameterName = "TimeoutGlobal";
            var timeoutParameter = await parameterRepository.GetByNameAsync(timeoutParameterName);
            if (timeoutParameter == null)
            {
                var newTimeoutParameter = new Parameter
                {
                    ParameterName = timeoutParameterName,
                    ParameterValue = "30",
                    ParameterDescription = "Tiempo de espera global en segundos para el sistema",
                    ParameterCategory = "Sistema",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await parameterRepository.CreateAsync(newTimeoutParameter);
            }

            const string ambienteParameterName = "IdentificacionAmbiente";
            var ambienteParameter = await parameterRepository.GetByNameAsync(ambienteParameterName);
            if (ambienteParameter == null)
            {
                var newAmbienteParameter = new Parameter
                {
                    ParameterName = ambienteParameterName,
                    ParameterValue = "ApiMaps",
                    ParameterDescription = "Identificación del ambiente de la aplicación",
                    ParameterCategory = "Sistema",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await parameterRepository.CreateAsync(newAmbienteParameter);
            }
            
            const string placesApiKeyName = "PlacesApiKey";
            var placesApiKey = await parameterRepository.GetByNameAsync(placesApiKeyName);
            if (placesApiKey == null)
            {
                var newPlacesKey = new Parameter
                {
                    ParameterName = placesApiKeyName,
                    ParameterValue = "YOUR_API_KEY_HERE",
                    ParameterDescription = "API Key de Google Places",
                    ParameterCategory = "MapServices",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await parameterRepository.CreateAsync(newPlacesKey);
            }
            
            const string aiEndpointName = "IAEndpoint";
            var aiEndpoint = await parameterRepository.GetByNameAsync(aiEndpointName);
            if (aiEndpoint == null)
            {
                var newParam = new Parameter
                {
                    ParameterName = aiEndpointName,
                    ParameterValue = "https://mi-api-ia.example.com",
                    ParameterDescription = "Endpoint para la IA de corrección de direcciones",
                    ParameterCategory = "AI",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await parameterRepository.CreateAsync(newParam);
            }
            
            const string aiApiKeyName = "ApiKeyIA";
            var aiApiKey = await parameterRepository.GetByNameAsync(aiApiKeyName);
            if (aiApiKey == null)
            {
                var newParam = new Parameter
                {
                    ParameterName = aiApiKeyName,
                    ParameterValue = "TU_CLAVE_API_IA_HERE",
                    ParameterDescription = "API Key por example para OpenAI (ChatGPT)",
                    ParameterCategory = "AI",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await parameterRepository.CreateAsync(newParam);
            }
            
            const string placesEndpoint = "PlacesEndpoint";
            var placesEndpointName = await parameterRepository.GetByNameAsync(placesEndpoint);
            if (placesEndpointName == null)
            {
                var newParam = new Parameter
                {
                    ParameterName = placesEndpoint,
                    ParameterValue = "https://maps.googleapis.com/maps/api/place/nearbysearch/json",
                    ParameterDescription = "Endpoint para google places",
                    ParameterCategory = "MapServices",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await parameterRepository.CreateAsync(newParam);
            }
            
            const string placesDetailsEndpoint = "PlacesDetailsEndpoint";
            var placesDetailsEndpointName = await parameterRepository.GetByNameAsync(placesDetailsEndpoint);
            if (placesDetailsEndpointName == null)
            {
                var newParam = new Parameter
                {
                    ParameterName = placesDetailsEndpoint,
                    ParameterValue = "https://maps.googleapis.com/maps/api/place/details/json",
                    ParameterDescription = "Endpoint para google places details",
                    ParameterCategory = "MapServices",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await parameterRepository.CreateAsync(newParam);
            }
            
            const string apiTraceEnabledParameterName = "ApiTraceEnabled";
            var apiTraceEnabledParameter = await parameterRepository.GetByNameAsync(apiTraceEnabledParameterName);
            if (apiTraceEnabledParameter == null)
            {
                var newApiTraceEnabledParameter = new Parameter
                {
                    ParameterName = apiTraceEnabledParameterName,
                    ParameterValue = "true",
                    ParameterDescription = "Determina si se debe registrar la traza de la API",
                    ParameterCategory = "Sistema",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await parameterRepository.CreateAsync(newApiTraceEnabledParameter);
            }
        }
    }
}