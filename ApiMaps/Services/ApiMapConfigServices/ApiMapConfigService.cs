using System.Reactive;
using System.Reactive.Linq;
using ApiMaps.Exceptions;
using ApiMaps.Models.DTOs.ApiMapConfigDtos;
using ApiMaps.Models.Entities;
using ApiMaps.Models.Repositories.ApiMapConfigRepositories;
using ApiMaps.Services.LoggingServices;

namespace ApiMaps.Services.ApiMapConfigServices
{
    /// <summary>
    /// Implementa el servicio para gestionar la configuración de proveedores de mapas de forma 100% reactiva.
    /// Aplica la lógica de negocio, validaciones y se comunica con el repositorio a través de observables.
    /// </summary>
    public class ApiMapConfigService : IApiMapConfigService
    {
        private readonly IApiMapConfigRepository _repository;
        private readonly ILoggerService<ApiMapConfigService> _logger;
        private readonly IServiceExecutor _serviceExecutor;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ApiMapConfigService"/>.
        /// </summary>
        /// <param name="repository">Repositorio para acceder a los datos de ApiMapConfig.</param>
        /// <param name="logger">Servicio de logging para registrar actividad y errores.</param>
        /// <param name="serviceExecutor">Ejecutor reactivo para manejar errores y suscripciones.</param>
        public ApiMapConfigService(
            IApiMapConfigRepository repository,
            ILoggerService<ApiMapConfigService> logger,
            IServiceExecutor serviceExecutor)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceExecutor = serviceExecutor ?? throw new ArgumentNullException(nameof(serviceExecutor));
        }

        /// <inheritdoc/>
        public IObservable<ApiMapConfig> CreateAsync(CreateApiMapConfigDto dto)
        {
            return _serviceExecutor.ExecuteAsync<ApiMapConfig>(() =>
            {
                _logger.LogInfo($"Iniciando creación de configuración para el proveedor: {dto.Proveedor}");
                
                if (string.IsNullOrWhiteSpace(dto.Proveedor))
                {
                    _logger.LogWarning("El proveedor es obligatorio.");
                    throw new ArgumentException("El proveedor es obligatorio.", nameof(dto.Proveedor));
                }
                if (string.IsNullOrWhiteSpace(dto.EndPoint))
                {
                    _logger.LogWarning("El endpoint es obligatorio.");
                    throw new ArgumentException("El endpoint es obligatorio.", nameof(dto.EndPoint));
                }
                if (string.IsNullOrWhiteSpace(dto.ApiKey))
                {
                    _logger.LogWarning("La API key es obligatoria.");
                    throw new ArgumentException("La API key es obligatoria.", nameof(dto.ApiKey));
                }

                var config = new ApiMapConfig
                {
                    Proveedor = dto.Proveedor,
                    EndPoint = dto.EndPoint,
                    ApiKey = dto.ApiKey,
                    EndPointParameter = dto.EndPointParameter,
                    Additional = dto.Additional,
                    Prioridad = dto.Prioridad,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                return Observable.FromAsync(async () =>
                {
                    var existingConfigs = await _repository.GetAllAsync();
                    if (existingConfigs.Any(c => c.Proveedor.Equals(config.Proveedor, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new InvalidOperationException($"Ya existe una configuración para el proveedor {config.Proveedor}");
                    }
                    await _repository.AddAsync(config);
                    return config;
                });
            });
        }

        /// <inheritdoc/>
        public IObservable<ApiMapConfig?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID debe ser mayor que 0.", nameof(id));
            }
            return _serviceExecutor.ExecuteAsync<ApiMapConfig?>(() =>
            {
                _logger.LogInfo($"Obteniendo configuración con ID: {id}");
                return Observable.FromAsync(() => _repository.GetByIdAsync(id));
            });
        }

        /// <inheritdoc/>
        public IObservable<IEnumerable<ApiMapConfig>> GetAllAsync()
        {
            return _serviceExecutor.ExecuteAsync<IEnumerable<ApiMapConfig>>(() =>
            {
                _logger.LogInfo("Obteniendo todas las configuraciones de proveedores de mapas.");
                return Observable.FromAsync(() => _repository.GetAllAsync());
            });
        }

        /// <inheritdoc/>
        public IObservable<ApiMapConfig> UpdateAsync(UpdateApiMapConfigDto dto)
        {
            return _serviceExecutor.ExecuteAsync<ApiMapConfig>(() =>
            {
                _logger.LogInfo($"Actualizando configuración con ID: {dto.Id}");
                if (dto.Id <= 0)
                {
                    _logger.LogWarning("El ID debe ser mayor que 0.");
                    throw new ArgumentException("El ID debe ser mayor que 0.", nameof(dto.Id));
                }
                if (string.IsNullOrWhiteSpace(dto.Proveedor))
                {
                    _logger.LogWarning("El proveedor es obligatorio.");
                    throw new ArgumentException("El proveedor es obligatorio.", nameof(dto.Proveedor));
                }
                if (string.IsNullOrWhiteSpace(dto.EndPoint))
                {
                    _logger.LogWarning("El endpoint es obligatorio.");
                    throw new ArgumentException("El endpoint es obligatorio.", nameof(dto.EndPoint));
                }
                if (string.IsNullOrWhiteSpace(dto.ApiKey))
                {
                    _logger.LogWarning("La API key es obligatoria.");
                    throw new ArgumentException("La API key es obligatoria.", nameof(dto.ApiKey));
                }
                return Observable.FromAsync(async () =>
                {
                    var existingConfig = await _repository.GetByIdAsync(dto.Id);
                    if (existingConfig == null)
                    {
                        _logger.LogWarning($"No se encontró la configuración con ID: {dto.Id}");
                        throw new ResourceNotFoundException($"La configuración con ID {dto.Id} no fue encontrada.");
                    }

                    existingConfig.Proveedor = dto.Proveedor;
                    existingConfig.EndPoint = dto.EndPoint;
                    existingConfig.ApiKey = dto.ApiKey;
                    existingConfig.EndPointParameter = dto.EndPointParameter;
                    existingConfig.Additional = dto.Additional;
                    existingConfig.Prioridad = dto.Prioridad;
                    existingConfig.UpdatedAt = DateTime.UtcNow;
                    existingConfig.UpdatedBy = "System";

                    await _repository.UpdateAsync(existingConfig);
                    return existingConfig;
                });
            });
        }

        /// <inheritdoc/>
        public IObservable<Unit> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID debe ser mayor que 0.", nameof(id));
            }
            return _serviceExecutor.ExecuteAsync<Unit>(() =>
            {
                _logger.LogInfo($"Eliminando configuración con ID: {id}");
                return Observable.FromAsync(async () =>
                {
                    var existingConfig = await _repository.GetByIdAsync(id);
                    if (existingConfig == null)
                    {
                        _logger.LogWarning($"No se encontró la configuración con ID: {id} para eliminar.");
                        throw new ResourceNotFoundException($"La configuración con ID {id} no fue encontrada.");
                    }
                    await _repository.DeleteAsync(id);
                    return Unit.Default;
                });
            });
        }
    }
}
