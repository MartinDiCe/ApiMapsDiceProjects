using System.Reactive.Linq;
using ApiMaps.Exceptions;
using ApiMaps.Models.DTOs.ParameterDtos;
using ApiMaps.Models.Entities;
using ApiMaps.Models.Repositories.ParameterRepositories;
using ApiMaps.Services.LoggingServices;

namespace ApiMaps.Services.ParameterServices
{
/// <summary>
    /// Implementación del servicio para la gestión de parámetros globales.
    /// Utiliza un <see cref="IServiceExecutor"/> para encapsular la lógica de ejecución reactiva
    /// y un servicio de logging para registrar la actividad.
    /// </summary>
    public class ParameterService : IParameterService
    {
        private readonly IParameterRepository _parameterRepository;
        private readonly ILoggerService<ParameterService> _logger;
        private readonly IServiceExecutor _serviceExecutor;

        /// <summary>
        /// Crea una nueva instancia de <see cref="ParameterService"/>.
        /// </summary>
        /// <param name="parameterRepository">Repositorio para acceder a los datos de la entidad <see cref="Parameter"/>.</param>
        /// <param name="logger">Servicio de logging para registrar actividad y errores.</param>
        /// <param name="serviceExecutor">Ejecutor reactivo para manejar errores y suscripciones.</param>
        /// <exception cref="ArgumentNullException">Si algún parámetro es nulo.</exception>
        public ParameterService(
            IParameterRepository parameterRepository,
            ILoggerService<ParameterService> logger,
            IServiceExecutor serviceExecutor)
        {
            _parameterRepository = parameterRepository ?? throw new ArgumentNullException(nameof(parameterRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceExecutor = serviceExecutor ?? throw new ArgumentNullException(nameof(serviceExecutor));
        }

        /// <inheritdoc />
        public IObservable<Parameter> CreateAsync(CreateParameterDto dto)
        {
            return _serviceExecutor.ExecuteAsync<Parameter>(() =>
            {
                _logger.LogInfo($"Iniciando la creación del parámetro global: {dto.ParameterName}");

                // Validaciones de entrada.
                if (string.IsNullOrWhiteSpace(dto.ParameterName))
                {
                    _logger.LogWarning("El nombre del parámetro es obligatorio.");
                    throw new ArgumentException("El nombre del parámetro es obligatorio.", nameof(dto.ParameterName));
                }
                if (string.IsNullOrWhiteSpace(dto.ParameterValue))
                {
                    _logger.LogWarning("El valor del parámetro es obligatorio.");
                    throw new ArgumentException("El valor del parámetro es obligatorio.", nameof(dto.ParameterValue));
                }
                
                if (string.IsNullOrWhiteSpace(dto.ParameterCategory))
                {
                    _logger.LogWarning("El valor del parámetro es obligatorio.");
                    throw new ArgumentException("La categoría del parámetro es obligatorio.", nameof(dto.ParameterValue));
                }

                var parameter = new Parameter
                {
                    ParameterName = dto.ParameterName,
                    ParameterValue = dto.ParameterValue,
                    ParameterDescription = dto.ParameterDescription,
                    ParameterCategory = dto.ParameterCategory,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                _logger.LogInfo($"Creando el parámetro: {parameter.ParameterName}");

                // Convertimos el Task en IObservable usando Observable.FromAsync.
                return Observable.FromAsync(() => _parameterRepository.CreateAsync(parameter));
            });
        }

        /// <inheritdoc />
        public IObservable<Parameter?> GetByIdAsync(int parameterId)
        {
            if (parameterId <= 0)
            {
                throw new ArgumentException("El ID del parámetro debe ser mayor que 0.", nameof(parameterId));
            }

            return _serviceExecutor.ExecuteAsync<Parameter?>(() =>
            {
                _logger.LogInfo($"Buscando parámetro con ID {parameterId}.");
                return Observable.FromAsync(() => _parameterRepository.GetByIdAsync(parameterId));
            });
        }

        /// <inheritdoc />
        public IObservable<Parameter?> GetByNameAsync(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException("El nombre del parámetro es obligatorio.", nameof(parameterName));
            }

            return _serviceExecutor.ExecuteAsync<Parameter?>(() =>
            {
                _logger.LogInfo($"Buscando parámetro con el nombre '{parameterName}'.");
                return Observable.FromAsync(() => _parameterRepository.GetByNameAsync(parameterName));
            });
        }

        /// <inheritdoc />
        public IObservable<Parameter> UpdateAsync(UpdateParameterDto dto)
        {
            return _serviceExecutor.ExecuteAsync<Parameter>(() =>
            {
                _logger.LogInfo($"Iniciando actualización del parámetro con ID: {dto.ParameterId}");
                
                if (dto.ParameterId <= 0)
                {
                    _logger.LogWarning("El ID del parámetro debe ser mayor que 0.");
                    throw new ArgumentException("El ID del parámetro debe ser mayor que 0.", nameof(dto.ParameterId));
                }

                if (string.IsNullOrWhiteSpace(dto.ParameterName))
                {
                    _logger.LogWarning("El nombre del parámetro es obligatorio.");
                    throw new ArgumentException("El nombre del parámetro es obligatorio.", nameof(dto.ParameterName));
                }

                if (string.IsNullOrWhiteSpace(dto.ParameterValue))
                {
                    _logger.LogWarning("El valor del parámetro es obligatorio.");
                    throw new ArgumentException("El valor del parámetro es obligatorio.", nameof(dto.ParameterValue));
                }

                if (string.IsNullOrWhiteSpace(dto.ParameterCategory))
                {
                    _logger.LogWarning("La categoría del parámetro es obligatoria.");
                    throw new ArgumentException("La categoría del parámetro es obligatoria.",
                        nameof(dto.ParameterCategory));
                }
                
                return Observable.FromAsync(async () =>
                {
                    var existingParameter = await _parameterRepository.GetByIdAsync(dto.ParameterId);
                    if (existingParameter == null)
                    {
                        _logger.LogWarning($"No se encontró el parámetro con ID: {dto.ParameterId}");
                        throw new ResourceNotFoundException($"El parámetro con ID {dto.ParameterId} no fue encontrado.");
                    }

                    existingParameter.ParameterName = dto.ParameterName;
                    existingParameter.ParameterValue = dto.ParameterValue;
                    existingParameter.ParameterDescription = dto.ParameterDescription;
                    existingParameter.ParameterCategory = dto.ParameterCategory;
                    existingParameter.UpdatedAt = DateTime.UtcNow;
                    existingParameter.UpdatedBy = "System";

                    _logger.LogInfo($"Actualizando el parámetro: {existingParameter.ParameterName}");
                    
                    await _parameterRepository.UpdateAsync(existingParameter);
                    return existingParameter;
                });
            });
        }

    }
}