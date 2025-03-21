﻿using ApiMaps.Models.Entities;

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